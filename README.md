# 🟨 MEHRAN // GAME DEV

> I make games about knights, chickens & dish soap. This is the website that holds all of them.

My personal game dev portfolio: a loud, paper-and-ink, **neo-brutalist** site with thick black borders, hard offset shadows, taped-on stickers, and zero patience for corporate polish. It is built with plain **HTML, CSS, and vanilla JavaScript**, with no framework and no build step.

🌐 **Live site:** https://mehran579.github.io/portfolio/

<img width="320" height="180" alt="Screenshot 2026-10-24 192452" src="https://github.com/user-attachments/assets/a7fe51cc-961d-43ec-8ea7-7222a4ea1cad" />
<img width="320" height="180" alt="Screenshot 2026-10-24 192524" src="https://github.com/user-attachments/assets/daeff792-61b3-4f9b-81fc-3717f0c8ee29" />
<img width="320" height="180" alt="image" src="https://github.com/user-attachments/assets/f13378d2-1411-41ba-91b9-bbee65ca9dcd" />


---

## ✨ What's On the Site

| Section | What it is |
| --- | --- |
| **Hero** | Who I am, four competing taglines, and a taped-on photo card with a "bug count: classified" |
| **01 / Technical centerpiece** | The flagship: *A Knight With 2 Guns: Trapped Without a Sun*, with an autoplaying clip, a spec grid, and links |
| **02 / Shipped games** | A card grid of the seven other games, each with a looping video, a hook, bullets, tags, and links |
| **03 / Unfinished on purpose** | The "Currently Cooking" prototypes: *Back of a Notebook*, *Desk Bud*, and *DA Beach* |
| **04 / The devlog entry** | The honest "boring stuff" about me, plus a skills list |
| **05 / Receipts** | Devlog and itch.io links |
| **06 / Final boss** | Contact links and a very serious call to action |

Every game also gets its **own project page** with a hero video, a "what I built" breakdown, and an interactive **project analysis flowchart** you can zoom and pan.

---

## 🎮 The Projects

| Project | Jam / context | Play | Code |
| --- | --- | --- | --- |
| **A Knight With 2 Guns** *(flagship)* | 3D third-person wave shooter | [itch.io](https://mehran13579.itch.io/a-knight-with-2-guns-trapped-without-a-sun) | [repo](https://github.com/Mehran579/3droguelikerepo) |
| **Farm Simulator** | IIIT N Game Jam: Tantrafiesta | [itch.io](https://mehran13579.itch.io/farm-simulator) | [repo](https://github.com/Mehran579/farm-simulator-repo-new) |
| **Wind & Physics Simulation** | Technical task | [Google Drive](https://drive.google.com/drive/folders/1KovreB3hnI7imilEqqm-iBtaDjDS-G8n) | [repo](https://github.com/Mehran579/Unity_Wind_Physics_Task_MdMehran) |
| **Inverted 101** | Geek N Jam | [itch.io](https://mehran13579.itch.io/inverted-101) | [repo](https://github.com/Mehran579/Inverted-101) |
| **Walking on Shells** | Club Club Jam Jam | [itch.io](https://mehran13579.itch.io/walking-on-shells) | [repo](https://github.com/Mehran579/walking-on-egg-shells) |
| **Marching Buddies** | Brainless Game Jam 2026 | [itch.io](https://mehran13579.itch.io/marching-buddies) | [repo](https://github.com/Mehran579/Marching-Buddies) |
| **Boxing 2.5D** | First hands with 3D | [itch.io](https://mehran13579.itch.io/boxing-unity-25d-2nd) | [repo](https://github.com/Mehran579/BOXXIng-2.5d) |
| **Dishwasher vs Dish** | Roguelite | [itch.io](https://mehran13579.itch.io/dish-vs-dishwasher) | [repo](https://github.com/Mehran579/DishWasherVsDish) |

**Currently cooking:** *Back of a Notebook* (a pen-drawn metroidvania), *Desk Bud* (a desktop 20-20-20 eye-rest buddy), and *DA Beach* (a troll game with a Pokémon-style fight nobody asked for).

---

## 🧱 How It's Built

- **No framework, no bundler, no npm.** Open the files in a browser and it works.
- **Data-driven cards.** Games live in a plain array in `js/games.js` and prototypes in `js/cooking.js`. Small renderers (`games-renderer.js`, `cooking-renderer.js`) turn them into cards, so adding a project is mostly editing one object.
- **Self-updating counts.** The hero and section headings ("Eight projects", "The other seven") are computed from the data by a tiny number-to-words function and written into `data-project-count` elements.
- **Media fallbacks.** The renderer picks a `<video>` or `<img>` from the file extension, and shows a "slot" placeholder if the media is missing or fails to load.
- **Project analysis viewer.** On each game page, the flowchart opens in a native `<dialog>` lightbox with **mouse wheel zoom, drag to pan, pinch zoom on touch, and zoom buttons** (`js/project-breakdown.js`). Zoom ranges from 1× to 6× and panning is clamped to the image bounds.
- **Responsive.** Layouts adapt at **800px** and **500px** breakpoints.
- **Cache busting** with `?v=` query strings on the stylesheet and scripts.

---

## 🎨 Design System

Everything is driven by CSS custom properties in `css/style.css`.

| Token | Value | Used for |
| --- | --- | --- |
| `--ink` | `#101014` | Text, borders, shadows |
| `--paper` | `#f4edda` | Page background |
| `--yellow` | `#f6d743` | Top bar, highlights, buttons |
| `--red` | `#ef4b35` | Flagship section, danger accents, kickers |
| `--cyan` | `#20c9c7` | Cooking section, "good" accents |
| `--white` | `#fffaf0` | Cards |
| `--muted` | `#635f55` | Secondary text |
| `--border` | `4px solid var(--ink)` | Every box |
| `--shadow` | `8px 8px 0 var(--ink)` | Hard offset shadow, never blurred |

**Fonts** (Google Fonts):

- **Barlow Condensed** (600 to 900): display headings, always uppercase
- **DM Mono** (400, 500): labels, tags, kickers, nav
- **Inter** (400 to 800): body text

**Signature details:** a faint SVG noise overlay across the whole page, a logo block tilted -2°, outlined (`.stroke`) headline words, taped-on sticker labels, and numbered eyebrows like `01 / technical centerpiece`.

---

## 🚀 Run It Locally

It is a static site, so any local server works.

```bash
git clone https://github.com/Mehran579/portfolio.git
cd portfolio

# pick one
python -m http.server 8000      # then open http://localhost:8000
# or use the "Live Server" extension in VS Code
```

> Opening `index.html` directly from the file system also works, but a local server is closer to how GitHub Pages serves it.

---

## ➕ Adding a Project

1. **Drop the media** into `assets/games/<slug>/` (a gameplay `.mp4` and the project analysis `.png`).
2. **Add an entry** to the `games` array in `js/games.js`:

   ```js
   {
     "num": "02H / SOME JAM",
     "slug": "my-new-game",
     "title": "My New Game",
     "hook": "One line that sells it.",
     "media": "assets/games/my-new-game/gameplay.mp4",
     "bullets": ["What it does", "Why it is cool"],
     "tags": ["UNITY", "C#", "2D"],
     "link": "https://your-name.itch.io/my-new-game",
     "gitRepo": "https://github.com/you/your-repo.git"
   }
   ```

3. **Copy an existing page** from `games/` to `games/<slug>.html` and edit the text, video, and breakdown image path.
4. The card, the project counts, and the "View project" link update automatically.

To add a prototype to the **Currently Cooking** page, add an object to `js/cooking.js` the same way.

---

## 📁 Project Structure

```
portfolio/
├── index.html               # Homepage
├── cooking.html             # Currently Cooking page
├── games/                   # One page per project
│   ├── knight.html          # Flagship
│   ├── farm-simulator.html
│   ├── wind-and-physics-simulation.html
│   ├── inverted-101.html
│   ├── walking-on-shells.html
│   ├── marching-buddies.html
│   ├── boxing-2-5d.html
│   └── dishwasher-vs-dish.html
├── css/
│   └── style.css            # The whole design system
├── js/
│   ├── games.js             # Project data
│   ├── games-renderer.js    # Builds the game cards and the project counts
│   ├── cooking.js           # Prototype data
│   ├── cooking-renderer.js  # Builds the cooking cards
│   ├── project-breakdown.js # Zoom and pan lightbox for flowcharts
│   └── main.js              # Reserved for site-wide interactions
└── assets/
    ├── me.jpg               # Hero photo
    └── games/<slug>/        # Gameplay videos and project analysis images
```

---

## 🌍 Deployment

Hosted for free on **GitHub Pages** straight from the repository. Push to the default branch and the site updates.

---

## 🛠️ Built With

- HTML5, CSS3 (custom properties, grid, flexbox, sticky header)
- Vanilla JavaScript (ES6, native `<dialog>`, Pointer Events)
- [Google Fonts](https://fonts.google.com/): Barlow Condensed, DM Mono, Inter
- A lot of caffeine and Unity brain rot

---

## 📬 Say Hi

Links to **itch.io**, **GitHub**, **Instagram**, **LinkedIn**, and email are in the footer of the live site.

> *Have a bug? Make it a feature.*

---

<sub>MEHRAN // SELF-TAUGHT // DELHI &nbsp;·&nbsp; © 2026 // MADE WITH UNITY BRAIN ROT</sub>
