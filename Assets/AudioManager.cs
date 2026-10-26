using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// IMPORTANT: add new names at the END of the SFX and Music lists. The Inspector stores each entry's id as a number,
// so inserting a name in the middle would shift every entry after it.
public enum SFX
{
    // Dialogue / UI
    DialogueBlip, DialogueAdvance, DialogueOpen, DialogueClose, UIClick,
    // Footsteps (one per level surface)
    FootstepWood, FootstepGrass, FootstepCobble, FootstepGround,
    // Story world
    DoorOpen, FadeWhoosh, ExclamationPop, SeedPickup, SeedPlant,
    BossKnockback, Rumble1, Rumble2, Rumble3, BossEmerge, ShockwaveBoom, GrassCrunch,
    SoldiersArrive, ArmorClank, KnightSpawn,
    // Player combat
    SwordSwing, SwordHit, Dash, PlayerHurt, PlayerDeath,
    // Enemies / waves
    EnemyHit, EnemyDeath, EnemyAttack, EnemySpawn, WaveCleared,
    // Boss fight
    BossRoar, MissileLaunch, MissileExplode, ShotCharge, ShotLock, ShotLaunch,
    ReflectClang, HazardWarning, HazardSpike, BossHurt, BossDeath,
    // Stingers
    BossSting, Victory, GameOver, AngelChime, AngelVanish
}

// None = "don't change the music". Handy as the default for a field that shows a dropdown in the Inspector.
public enum Music
{
    None,
    House, Farm, Market, MarketTense, War, Battle, Boss, Angel
}

[System.Serializable]
public class SoundEntry
{
    public SFX id;
    public AudioClip[] clips;                    // more than one = random pick each play
    [Range(0f, 1f)] public float volume = 1f;
    public float pitchVariation = 0.08f;         // random +/- pitch so repeats don't sound identical
    public float minInterval = 0.04f;            // stops the same sound stacking / playing too often
}

[System.Serializable]
public class MusicEntry
{
    public Music id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;    // per-track volume, multiplied with the overall Music Volume
}

// Put this on a ROOT GameObject in the FIRST scene only.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] AudioSource musicSource;    // optional: one is created for you if left empty
    [SerializeField] MusicEntry[] tracks;
    [SerializeField] bool playStartingTrack = true;
    [SerializeField] Music startingTrack = Music.House;   // plays once, when the game first starts
    [Range(0f, 1f)]
    [SerializeField] float musicVolume = 0.5f;   // overall music volume
    [SerializeField] float defaultFadeTime = 1f; // used when you don't pass a fade time

    [Header("SFX")]
    [Range(0f, 1f)]
    [SerializeField] float sfxVolume = 1f;
    [SerializeField] int voices = 12;            // how many sounds can play at once
    [SerializeField] SoundEntry[] sounds;

    [Header("Lifetime")]
    [SerializeField] int lastSceneBuildIndex = 2;   // 0 = first scene, 2 = third scene; loading anything after it removes the manager

    readonly Dictionary<SFX, SoundEntry> map = new Dictionary<SFX, SoundEntry>();
    readonly Dictionary<Music, MusicEntry> trackMap = new Dictionary<Music, MusicEntry>();
    readonly Dictionary<SFX, float> lastPlayed = new Dictionary<SFX, float>();
    AudioSource[] pool;
    int nextVoice;

    AudioSource activeMusic;     // the music source that is currently audible / fading in
    AudioSource otherMusic;      // the spare one, used for crossfading
    Coroutine musicRoutine;
    bool musicStopping;

    void Awake()
    {
        // Returning to the first scene would create a second manager, so the extra one removes itself
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Lookups
        if (sounds != null)
            foreach (SoundEntry s in sounds) map[s.id] = s;
        if (tracks != null)
            foreach (MusicEntry m in tracks) trackMap[m.id] = m;

        // SFX voice pool (each source keeps its own pitch, so overlapping sounds don't affect each other)
        pool = new AudioSource[voices];
        for (int i = 0; i < voices; i++)
        {
            pool[i] = gameObject.AddComponent<AudioSource>();
            pool[i].playOnAwake = false;
            pool[i].spatialBlend = 0f;
        }

        // Music: two sources so tracks can crossfade
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

        AudioSource spare = gameObject.AddComponent<AudioSource>();
        spare.outputAudioMixerGroup = musicSource.outputAudioMixerGroup;

        foreach (AudioSource src in new[] { musicSource, spare })
        {
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            src.loop = true;
            src.volume = 0f;
        }

        activeMusic = musicSource;
        otherMusic = spare;

        if (playStartingTrack) PlayMusic(startingTrack, 0f);   // 0 = start instantly, no fade-in
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single && scene.buildIndex > lastSceneBuildIndex)
            Destroy(gameObject);
    }

    // ------------------------------------------------------------ SFX

    // AudioManager.Play(SFX.SwordSwing);
    public static void Play(SFX id)
    {
        if (Instance != null) Instance.PlayInternal(id);
    }

    void PlayInternal(SFX id)
    {
        if (!map.TryGetValue(id, out SoundEntry s) || s.clips == null || s.clips.Length == 0) return;

        float now = Time.unscaledTime;   // unscaled, so a hit stop can't break the interval check
        if (lastPlayed.TryGetValue(id, out float last) && now - last < s.minInterval) return;
        lastPlayed[id] = now;

        AudioSource src = pool[nextVoice];
        nextVoice = (nextVoice + 1) % pool.Length;

        src.clip = s.clips[Random.Range(0, s.clips.Length)];
        src.volume = s.volume * sfxVolume;
        src.pitch = 1f + Random.Range(-s.pitchVariation, s.pitchVariation);
        src.Play();
    }

    // ------------------------------------------------------------ Music

    // Crossfades to a track from the Tracks list. Does nothing for Music.None or if that track is already playing.
    // AudioManager.Instance.PlayMusic(Music.Farm);        -> fades over Default Fade Time
    // AudioManager.Instance.PlayMusic(Music.Farm, 0f);    -> instant switch
    public void PlayMusic(Music id, float fade = -1f)
    {
        if (id == Music.None) return;
        if (!trackMap.TryGetValue(id, out MusicEntry entry) || entry.clip == null) return;

        if (!musicStopping && activeMusic.clip == entry.clip && activeMusic.isPlaying) return;

        musicStopping = false;
        if (fade < 0f) fade = defaultFadeTime;

        if (musicRoutine != null) StopCoroutine(musicRoutine);
        musicRoutine = StartCoroutine(Crossfade(entry.clip, musicVolume * entry.volume, fade));
    }

    // Fades the music out to silence
    public void StopMusic(float fade = -1f)
    {
        if (fade < 0f) fade = defaultFadeTime;

        musicStopping = true;
        if (musicRoutine != null) StopCoroutine(musicRoutine);
        musicRoutine = StartCoroutine(FadeOutAll(fade));
    }

    // Fades use unscaled time so a hit stop (timeScale 0) can't freeze them
    IEnumerator Crossfade(AudioClip clip, float targetVolume, float fade)
    {
        AudioSource from = activeMusic;
        AudioSource to = otherMusic;
        activeMusic = to;                  // swap now, so an interruption still behaves sensibly
        otherMusic = from;

        to.Stop();                         // cut whatever the spare was still doing
        to.clip = clip;
        to.volume = 0f;
        to.Play();

        float fromStart = from.volume;
        float t = 0f;
        while (t < fade)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fade;
            from.volume = Mathf.Lerp(fromStart, 0f, k);
            to.volume = Mathf.Lerp(0f, targetVolume, k);
            yield return null;
        }

        from.Stop();
        from.volume = 0f;
        to.volume = targetVolume;
        musicRoutine = null;
    }

    IEnumerator FadeOutAll(float fade)
    {
        float activeStart = activeMusic.volume;
        float otherStart = otherMusic.volume;

        float t = 0f;
        while (t < fade)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fade;
            activeMusic.volume = Mathf.Lerp(activeStart, 0f, k);
            otherMusic.volume = Mathf.Lerp(otherStart, 0f, k);
            yield return null;
        }

        activeMusic.Stop();
        otherMusic.Stop();
        activeMusic.volume = 0f;
        otherMusic.volume = 0f;
        musicRoutine = null;
    }
}