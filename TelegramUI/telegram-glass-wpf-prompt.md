# MASTER PROMPT — "Aurora Glass" Telegram-Style Desktop Messenger UI (WPF / XAML / MVVM)

> Paste this whole file into Claude Code as the task brief. Work in phases (see section 16), commit after every phase, and do not skip the acceptance checklist (section 17).

---

## 0. ROLE & GOAL

You are a senior WPF/XAML engineer and a principal UI/UX designer in one person.
Build a **UI-only, high-fidelity Telegram-style desktop messenger client** called **"Aurora Glass"** using **WPF (.NET 8, C# 12, XAML)** with a **strict MVVM** architecture.

- There is **no real Telegram/network backend**. Use an in-memory **mock data layer** behind interfaces (`IChatService`, `IContactService`, ...) so a real backend can replace it later without touching ViewModels or Views.
- The visual identity is a **premium glassmorphism + soft 3D** theme: frosted translucent panels, layered depth, parallax, subtle perspective tilt, glowing accents, smooth physical-feeling animation.
- Quality bar: portfolio-grade, Apple/Windows 11 Fluent level polish, but with a distinctive and creative style that does not look like a stock template.

---

## 1. TECH STACK & CONSTRAINTS

| Topic | Decision |
|---|---|
| Framework | .NET 8, WPF, C# 12, nullable enabled, implicit usings |
| MVVM | `CommunityToolkit.Mvvm` (source generators: `[ObservableProperty]`, `[RelayCommand]`, `WeakReferenceMessenger`) |
| DI / Host | `Microsoft.Extensions.Hosting` + `Microsoft.Extensions.DependencyInjection` |
| Audio | `NAudio` (record + playback for voice messages) |
| Video | `LibVLCSharp.WPF` (preferred) or `MediaElement` fallback behind `IMediaPlayerService` |
| Images | `System.Windows.Media.Imaging` with decode-pixel-width caching; optional `WpfAnimatedGif` |
| Icons | Segoe Fluent Icons / Segoe MDL2 + custom `Geometry` resources (no raster icons) |
| Fonts | Inter or Segoe UI Variable (embedded), monospace: Cascadia Code |
| Persistence | Settings (theme/customization) as JSON in `%AppData%/AuroraGlass/settings.json` |
| Tests | xUnit + FluentAssertions for ViewModels and services |
| Code-behind | **Forbidden** except pure view-only concerns (e.g., `InitializeComponent`). Use behaviors / attached properties / triggers instead. |
| Third-party UI kits | Do **not** use MaterialDesign/MahApps. Build all styles/controls from scratch for full creative control. |

Hard rules:
1. No business logic in Views. No `MessageBox` in VMs (use `IDialogService`).
2. Every interactive visual state (Normal, Hover, Pressed, Focused, Disabled, Selected) must be styled.
3. All colors, brushes, radii, spacing, durations come from **resource dictionaries / design tokens** — never hard-coded in views.
4. UI must stay at 60 FPS with 10,000 messages (virtualization, frozen freezables, cached bitmaps).

---

## 2. DESIGN LANGUAGE: "AURORA GLASS"

### 2.1 Concept
A dark aurora-like gradient world (deep indigo → violet → cyan glow) with floating **frosted glass slabs**. Panels feel like thick glass sheets stacked in Z-space: each layer has its own blur, tint, edge highlight and shadow. Accent elements (avatars, send button, badges, recording orb) have soft **3D depth** (inner highlight + cast shadow + subtle tilt on hover).

### 2.2 Glass recipe (how to achieve it in WPF — be technically honest)
WPF has **no native backdrop blur**. Implement a reusable `GlassPanel` custom control with this strategy, in order of preference:

1. **Windows 11 system backdrop** via `DwmSetWindowAttribute` (`DWMWA_SYSTEMBACKDROP_TYPE` = Mica/Acrylic/Tabbed) + `WindowChrome` with `GlassFrameThickness = -1` for the main window shell. Detect OS support and fall back automatically.
2. **In-app fake backdrop blur** (works everywhere): a `BackdropLayer` renders the shared animated wallpaper once into a `VisualBrush`; each `GlassPanel` paints a **clipped, offset-aligned copy** of that brush with a `BlurEffect` (radius from `GlassBlurRadius` token), then overlays tint + noise + edge highlight. Alignment must follow the panel's position (use `TransformToAncestor`) so the blur looks like it's truly behind the glass.
3. **Fallback tier** (low-end GPU / "Reduce transparency" setting): solid tinted brush with a gradient, no blur.

Glass layers (bottom → top) for every `GlassPanel`:
1. Blurred backdrop copy (blur radius 24–40 px).
2. Tint fill: `GlassTint` with opacity 0.10–0.28 (token-driven).
3. Noise texture overlay (tiled 128×128 PNG, 3–4 % opacity) for realism — embed as resource.
4. Inner top-left **specular highlight**: `LinearGradientBrush` white 22 % → transparent over the top 40 %.
5. **Border**: 1 px gradient stroke (white 40 % at top-left → white 6 % at bottom-right), plus an optional outer 1 px dark hairline.
6. Drop shadow: `DropShadowEffect` BlurRadius 32, ShadowDepth 0–12, Opacity 0.35, color = deep indigo (not pure black). Prefer cheaper pre-rendered shadow bitmaps on large panels for performance.

### 2.3 Soft 3D recipe
- **Card tilt / parallax**: attached behavior `Tilt3D` using `PerspectiveTransform`-like effect. Because WPF lacks a perspective projection on 2D elements, implement with either (a) `Viewport2DVisual3D` + `PerspectiveCamera` hosting interactive content for hero elements (profile card, theme preview card, recording orb), or (b) lightweight fake 3D via `SkewTransform` + `ScaleTransform` + animated shadow offset for list items. Max tilt ±6°, spring-back easing.
- **Depth shadows**: 3 elevation levels (see tokens) — shadows grow and blur more with elevation.
- **Avatars**: circular, 3D "bead" look: radial highlight at top-left, inner shadow at bottom-right, 2 px glass ring, status dot with glow.
- **Buttons (primary)**: pill/orb with gradient fill, bevel highlight, glow on hover, press = scale 0.94 + shadow shrinks (feels physically pressed).
- **Message bubbles**: subtle gradient + glass edge, tail drawn with a `Path`, slight elevation shadow; own bubbles use the accent gradient.
- **Parallax wallpaper**: wallpaper layer shifts 6–12 px opposite to mouse movement (toggle in settings, off if "Reduce motion").
- **Ambient orbs**: 3–4 large blurred gradient blobs slowly drifting behind the glass (storyboard, 40–60 s loops, `RepeatBehavior=Forever`, only animate when window is active).

---

## 3. DESIGN TOKENS

### 3.1 Color palette (Dark "Aurora Night" — default)

| Token | Hex | Usage |
|---|---|---|
| `Bg.Base` | `#0B0B1A` | Window base |
| `Bg.Gradient.Start` | `#14123A` | Wallpaper gradient top-left |
| `Bg.Gradient.Mid` | `#2A1B5C` | Wallpaper mid |
| `Bg.Gradient.End` | `#0E3A5C` | Wallpaper bottom-right |
| `Orb.Violet` | `#7C4DFF` | Ambient orb |
| `Orb.Cyan` | `#22D3EE` | Ambient orb |
| `Orb.Pink` | `#FF4FA3` | Ambient orb |
| `Glass.Tint` | `#FFFFFF` @ 8–14 % | Panel tint |
| `Glass.TintStrong` | `#FFFFFF` @ 18–24 % | Elevated panels, menus |
| `Glass.Border.Light` | `#FFFFFF` @ 40 % | Top-left edge |
| `Glass.Border.Dark` | `#FFFFFF` @ 6 % | Bottom-right edge |
| `Accent.Primary` | `#7C6CFF` | Main accent |
| `Accent.Secondary` | `#22D3EE` | Gradient partner, links |
| `Accent.Gradient` | `#7C6CFF → #4FC3F7` (135°) | Own bubble, send button |
| `Accent.Glow` | `#7C6CFF` @ 55 % | Glow shadows |
| `Text.Primary` | `#F5F7FF` | Main text |
| `Text.Secondary` | `#B4B9D6` | Subtitles, timestamps |
| `Text.Tertiary` | `#7F85A8` | Hints, disabled |
| `Bubble.Incoming` | `#FFFFFF` @ 12 % over glass | Incoming bubble |
| `Bubble.Outgoing` | `Accent.Gradient` | Outgoing bubble |
| `Success` | `#34D399` | Online, delivered |
| `Warning` | `#FBBF24` | Pending, muted |
| `Danger` | `#FB7185` | Errors, delete, recording |
| `Badge.Unread` | `#7C6CFF` | Unread count |
| `Badge.Muted` | `#6B7194` | Muted unread |
| `Selection` | `#7C6CFF` @ 28 % | Selected row/message |

### 3.2 Light theme "Frost Day"
Base `#EAF0FF`, gradient `#EAF0FF → #D9E4FF → #F3E8FF`, glass tint = white @ 45–60 %, text primary `#141833`, secondary `#4A5078`, accent `#5B4BFF`, shadows color `#2A1B5C` @ 18 %. Provide full parity for every token.

### 3.3 Additional presets (selectable in customization)
- **Neon Tokyo**: bg `#0A0014`, accent `#FF2E97 → #00F0FF`.
- **Forest Mist**: bg `#07130F`, accent `#34D399 → #A3E635`.
- **Sunset Amber**: bg `#1A0B12`, accent `#FF8A4C → #FF4F81`.
- **Mono Ink**: grayscale glass, accent white.
- **Custom**: user picks accent + 2 gradient stops (color picker).

### 3.4 Typography

| Style | Font | Size / Weight / Line |
|---|---|---|
| `Display` | Inter | 28 / SemiBold / 36 |
| `Title` | Inter | 20 / SemiBold / 28 |
| `Subtitle` | Inter | 16 / Medium / 24 |
| `Body` | Inter | 14 / Regular / 20 |
| `BodyStrong` | Inter | 14 / SemiBold / 20 |
| `Caption` | Inter | 12 / Regular / 16 |
| `Micro` (timestamps, badges) | Inter | 11 / Medium / 14 |
| `Mono` | Cascadia Code | 13 / Regular / 18 |

Message font size scales by user setting (12–20 px). Enable `TextOptions.TextFormattingMode="Display"` and `TextRenderingMode="ClearType"` where it helps crispness on glass; support RTL (Persian/Arabic) with per-message `FlowDirection` auto-detect.

### 3.5 Spacing, radius, borders, elevation (4-pt grid)

| Token | Value |
|---|---|
| `Space.1` | 4 |
| `Space.2` | 8 |
| `Space.3` | 12 |
| `Space.4` | 16 |
| `Space.5` | 20 |
| `Space.6` | 24 |
| `Space.8` | 32 |
| `Space.10` | 40 |
| `Radius.XS` | 6 (chips, tags) |
| `Radius.S` | 10 (inputs, small buttons) |
| `Radius.M` | 14 (list items, menus) |
| `Radius.L` | 20 (cards, bubbles: 18 + 4 tail corner) |
| `Radius.XL` | 28 (main panels) |
| `Radius.Full` | 999 (pills, avatars) |
| `Border.Hairline` | 1 |
| `Border.Focus` | 2 (accent, outer glow 6 px) |
| `Elevation.1` | blur 12, depth 2, op 0.25 |
| `Elevation.2` | blur 24, depth 6, op 0.32 |
| `Elevation.3` | blur 40, depth 12, op 0.40 |

Sizes:
- Min window: **1000 × 640**, default **1280 × 800**, remember size/position/state.
- Custom title bar height **44**, caption buttons 46×32.
- Left **nav rail** width **72** (icons 24, hit target 48×48).
- **Chat list panel** width **340** (resizable 280–460 via `GridSplitter` styled as a thin glowing handle).
- **Chat panel**: flexible. Header height **64**, composer min height **56** (grows to max 160 for multi-line).
- **Info panel** (right): width **320**, slide-in/out 280 ms.
- Chat list item height **72**: avatar 52, gap 12, padding 12×10.
- Contacts item height **64**: avatar 44.
- Message bubble: max width **min(70 % of chat width, 520)**, padding 12×8, inner gap 2, grouped bubbles gap 2, new group gap 10.
- Media bubble: max 360×420, radius 18, 2 px inner padding, caption below.
- Composer buttons: 40×40 (send button 44×44 orb).
- Minimum hit target everywhere: 32×32 (mouse), 40×40 for primary actions.

### 3.6 Motion tokens

| Token | Value |
|---|---|
| `Motion.Fast` | 120 ms (hover, press) |
| `Motion.Base` | 220 ms (state changes, tab switch) |
| `Motion.Slow` | 360 ms (panel slide, page transition) |
| `Motion.Ambient` | 40–60 s (background drift) |
| Easing standard | `CubicEase EaseOut` |
| Easing spring | `ElasticEase` (Oscillations 1, Springiness 6) for pop-in only |
| Easing emphasized | `QuinticEase EaseInOut` for panel moves |

Respect `SystemParameters.ClientAreaAnimation`; provide a global `ReduceMotion` flag that collapses durations to 0 and disables ambient/parallax/tilt.

---

## 4. UX STRUCTURE & INFORMATION ARCHITECTURE

### 4.1 Window layout (3-pane + optional 4th)

```
┌────────────────────────────────────────────────────────────────────────┐
│ Custom Title Bar (drag, min/max/close, app title, global search)      │
├──────┬──────────────────┬───────────────────────────────┬─────────────┤
│ Nav  │  Chat List Panel │        Chat Panel             │ Info Panel  │
│ Rail │  - Search        │  Header (avatar/name/status)  │ (collapsible│
│ 72px │  - Folder Tabs   │  Messages (virtualized)       │  320px)     │
│      │  - Chat items    │  Typing indicator             │ - Profile   │
│      │                  │  Composer (attach/voice/send) │ - Media     │
│      │                  │                               │ - Files     │
└──────┴──────────────────┴───────────────────────────────┴─────────────┘
```

Responsive rules (window width): ≥1280 all panes; 1000–1279 info panel overlays as a drawer; <1000 not allowed (min width) but chat list can collapse to 72 px icon-only mode via toggle.

### 4.2 Navigation rail items
Chats (default) · Contacts · Calls (placeholder) · Saved Messages · Customization (palette icon) · Settings (bottom) · User avatar (bottom, opens profile flyout). Active item: glass "pill" indicator with accent glow that **slides** between items (animated `Canvas.Top`/`TranslateTransform`).

### 4.3 Core flows
1. **Open chat** → click item → chat panel cross-fade + messages stagger-in (max 12 items, 30 ms stagger) → scroll to last unread / bottom.
2. **Send text** → Enter sends, Shift+Enter newline → optimistic bubble appears with "sending" clock → "sent" ✓ → "delivered" ✓✓ → "read" ✓✓ (accent).
3. **Attach** → clip button opens glass popup menu (Photo/Video, File, Contact, Poll-placeholder, Location-placeholder). Drag & drop and Ctrl+V paste image also supported.
4. **Voice** → hold mic (or click to toggle) → recording UI replaces composer (see 6.3).
5. **Reply / Forward / Edit / Delete / React** via right-click context menu or hover action bar.
6. **Search** → global search (Ctrl+K) opens a glass command-palette-style overlay with chats, contacts, messages sections.
7. **Customize** → live-preview panel (see section 9).

### 4.4 States every screen must handle
Empty (illustrated with 3D glass shapes + CTA) · Loading (skeletons) · Error (inline retry card) · Offline banner ("Connecting…" with animated dots) · Partial-load (pagination spinner at top of history) · Disabled controls · Long text / long names (trim with tooltip) · RTL.

### 4.5 Accessibility & input
- Full keyboard navigation (Tab order, arrow keys in lists, Esc closes overlays, Enter/Space activates).
- `AutomationProperties.Name` on all icon buttons; UI Automation friendly.
- Focus visuals: 2 px accent outline + glow, never removed.
- Minimum contrast ≥ 4.5:1 for body text over glass (verify with worst-case wallpaper; adjust tint if needed).
- High contrast / reduce transparency switch.
- Shortcuts: `Ctrl+K` search, `Ctrl+N` new chat, `Ctrl+,` settings, `Ctrl+Shift+T` toggle theme, `Alt+↑/↓` prev/next chat, `Esc` close/cancel reply, `Ctrl+E` edit last message, `Ctrl+Enter` alt-send mode, `F11` full screen.

---

## 5. COMPONENT LIBRARY (build as reusable custom controls / styles)

Create in `Aurora.Controls` + `Aurora.Themes`:

| Control | Notes |
|---|---|
| `GlassPanel` | Section 2.2; DPs: `BlurRadius`, `TintOpacity`, `Elevation`, `CornerRadius`, `ShowNoise`, `ShowHighlight` |
| `GlassButton` | variants: Primary(gradient), Secondary(glass), Ghost, Icon, Danger; press-scale, glow |
| `OrbButton` | Round 3D button (send, mic, FAB) with bevel + glow |
| `GlassTextBox` | Floating label, clear button, focus glow, error state |
| `SearchBox` | Glass pill, animated icon, shortcut hint chip |
| `Avatar3D` | Image/initials fallback (deterministic gradient from name hash), status dot, story ring, size variants 24/32/44/52/96 |
| `Badge` | Unread (pop animation on change), muted, mention `@` |
| `GlassTabBar` / `FolderTabs` | Scrollable, sliding glass indicator, drag-reorder, badge per tab, overflow chevron |
| `ChatListItem` | See 6.5 |
| `MessageBubble` | Templates for text / photo / album / video / voice / file / system / reply / forwarded |
| `WaveformControl` | Renders bars from `float[]` peaks; playback progress overlay; draggable seek |
| `ProgressRing` | Determinate/indeterminate, glass track, gradient arc |
| `ShimmerSkeleton` | Animated gradient sweep (1.2 s loop) |
| `TypingIndicator` | 3 bouncing glass dots, staggered |
| `GlassMenu` / `ContextMenu` | Custom `ControlTemplate`, blur backdrop, slide+fade 120 ms, icons, shortcuts text, separators |
| `GlassToolTip` | Delay 400 ms |
| `Toast` / `Snackbar` | Stack bottom-center, auto-dismiss, undo action |
| `Dialog` host | Dimmed backdrop + glass dialog, focus trap |
| `ToggleSwitch` | Glass track, 3D thumb |
| `Slider` (glass) | For blur/depth/font settings, glowing thumb |
| `ColorPickerLite` | Palette swatches + HSV picker + hex input |
| `ImageViewer` | Fullscreen overlay: zoom (wheel/pinch), pan, swipe/arrow next/prev, thumbnails strip, close on Esc |
| `Tilt3DHost` | Viewport2DVisual3D wrapper for hero elements |
| `Divider` | Gradient hairline |
| `EmptyState` | Illustration + title + message + CTA |

---

## 6. FEATURE SPECIFICATIONS

### 6.1 Sending text messages
- Multiline composer (auto-grow to 160 px then scroll), placeholder "Write a message…", emoji button (open simple emoji panel with categories & search; `Segoe UI Emoji`), draft persistence per chat.
- Rich display: links (clickable, accent underline on hover), `@mention`, `#hashtag`, bold/italic/code/pre via simple markdown subset (`**b**`, `__i__`, `` `code` ``, ```` ```pre``` ````), emoji-only messages rendered large (3×).
- Message states enum: `Pending, Sent, Delivered, Read, Failed`. Failed shows red icon + tap to retry.
- Edit mode: composer shows "Editing" banner with original text; Esc cancels.
- Reply mode: banner with quoted snippet + thumbnail; click quote in bubble scrolls & flashes target message.
- Timestamps: 24h by default (setting), grouped by date separators ("Today", "Yesterday", "Mon, 12 Oct").
- Bubble grouping: consecutive messages from same sender within 2 min share avatar/tail; tail only on last in group.
- Hover action bar (floating glass pill above bubble): React, Reply, Forward, More.
- Reactions: row of glass chips under bubble with count; quick-react popover (6 emojis), pop animation.
- Selection mode: multi-select with checkboxes, top bar shows count + Forward/Delete/Copy.
- Scroll: "Jump to bottom" floating orb with unread count; "New messages" divider line; smooth scroll animation.
- Message search inside chat (Ctrl+F) with highlight and prev/next.

### 6.2 Photos
- Single photo bubble with rounded corners, blur-up placeholder (tiny blurred thumbnail → sharp image fade-in 220 ms).
- Albums (2–10 photos): Telegram-style mosaic grid algorithm (aspect-ratio aware, 2 px gaps) in a custom `MosaicPanel`.
- Send flow: picker (OpenFileDialog + drag-drop + clipboard) → **Preview sheet** (glass modal): thumbnails strip, caption box, "Send as file" toggle, crop/rotate (basic) placeholders, Send button.
- Upload progress: circular `ProgressRing` over image with cancel (×); then fades out.
- Click opens `ImageViewer` (zoom, pan, gallery navigation, save-as, share-placeholder).
- Decode with `DecodePixelWidth` ≤ 800 for bubbles; full-size only in viewer; cache with `IImageCache` (LRU, 200 MB cap).

### 6.3 Voice messages
**Recording UX**
- Press & hold mic orb (or click to toggle "locked" mode). While recording, composer morphs into a glass recording bar: pulsing red dot, timer `00:07`, live waveform (bars scrolling right→left from mic level), "◀ Slide to cancel" hint, **swipe up to lock**.
- Mic orb grows to 1.4× with breathing glow ring driven by input level.
- Release to send; slide left > 80 px cancels (bar shakes, trash icon animation, haptic-like pulse); locked mode shows Stop/Cancel/Send buttons.
- Min length 0.5 s else discard with toast "Hold to record".
- Max length 15 min; warn at 14:30.
- Implementation: `IVoiceRecorder` (NAudio `WaveInEvent`, 16-bit mono 48 kHz → encode Opus/OGG or WAV placeholder), emits `AudioLevel` events (30 Hz) and final peaks array (64–100 samples).
**Playback UX**
- Bubble: play/pause orb, waveform (progress colored with accent gradient, rest dimmed), duration/remaining time, playback speed chip (1× / 1.5× / 2×), unread-listened dot.
- Seek by dragging waveform; auto-play next voice message in chat (setting).
- Single global `IAudioPlaybackService` guarantees only one audio plays at a time.

### 6.4 Videos
- Bubble with thumbnail (first frame), central glass play orb, duration chip (bottom-left), size chip while not downloaded, download ring state machine: `NotDownloaded → Downloading(progress) → Ready → Playing`.
- Inline preview-on-hover (muted 3-sec loop) — optional setting.
- Click → fullscreen **VideoViewer**: custom glass transport bar (play/pause, scrubber with buffered range, volume with glass slider, speed, fullscreen, PiP-placeholder), keyboard (Space, ←/→ 5 s, ↑/↓ volume, F fullscreen, Esc exit), auto-hide controls after 2.5 s of idle mouse.
- Round "video note" bubble variant (circle 240 px) — optional stretch goal.
- Send flow same as photos (preview sheet with trim-placeholder, caption, compress toggle).

### 6.5 Chat list & chat system
`ChatListItem` layout (72 px): `[Avatar 52][Title (BodyStrong, trim) ········ Time (Micro)] / [Last message preview (Body Secondary, 1 line) ···· Badge/Pin/Mute/Status]`
- Last message prefix: "You: ", sender name for groups, icons for media ("📷 Photo", "🎤 0:23", "🎬 Video").
- States: Hover (glass lift + tilt 2°), Selected (accent-tinted glass + left glow bar), Pinned (pin icon, sticks top), Muted (dim badge, speaker-off icon), Typing (replace preview with animated "typing…" in accent), Unread (bold title/preview + badge), Draft ("Draft: …" in danger color).
- Swipe/hover quick actions: Pin, Mute, Archive, Delete (reveal on hover on right side; context menu with same + Mark as unread).
- Chat types: Private, Group, Channel, Bot, Saved Messages. Type-specific header subtitle ("online", "last seen recently", "128 members, 12 online", "2.4K subscribers").
- Create new chat/group wizard (placeholder dialog): select contacts (multi-chip input), name, avatar.
- Pagination: load older messages when scrolled to top (show shimmer rows), keep scroll anchor stable.
- Virtualization: `VirtualizingStackPanel` with `VirtualizationMode="Recycling"` and `ScrollUnit="Pixel"`; message list inverted/anchored to bottom (custom panel or reversed-transform technique) — document the approach chosen.

### 6.6 Contacts
- Alphabetical grouped list (sticky letter headers, A–Z jump rail on the right edge with magnetic hover), search with highlight, filter chips (All / Online / Recent / Favorites).
- Contact item: Avatar 44, name, last seen / status, quick actions on hover (Message, Call-placeholder, Favorite star).
- Contact profile page (in info panel or dialog): large Avatar3D (96, Tilt3D), name, phone, username, bio, shared media tabs, mutual groups, buttons (Message, Mute, Block, Delete) with confirmation dialog.
- Add contact dialog: name + phone with validation (inline errors), success toast.
- Contacts sync "loading" state with skeletons + pull-to-refresh style button.

### 6.7 Chat folder tabs (Telegram-style)
Tabs: **All · Unread · Personal · Groups · Channels · Bots · (custom folders)**.
- Horizontally scrollable glass tab bar above the list; active tab has sliding glass pill + glow; unread badge per tab (collapses to dot when narrow).
- Right-click tab → Edit folder, Reorder, Remove; "+" tab opens folder editor (name, icon, include/exclude chats & types).
- Drag & drop to reorder tabs (animated gap), drag a chat onto a tab to add it to that folder.
- Filtering implemented with `ICollectionView` (CollectionViewSource) and live shaping — **no** manual list rebuilds.
- Tab switch animation: list cross-fade + 8 px vertical slide (Motion.Base).

### 6.8 Loading system (global)
- **App splash**: logo orb forming from glass shards (3D rotation), progress arc, then shell cross-fades in (≤ 1.2 s).
- **Skeletons** for chat list (8 rows), message history (alternating left/right bubbles), contacts, profile.
- **Shimmer**: shared single storyboard/`ShimmerBrush` resource to avoid N animations.
- **Inline states**: send clock, upload/download rings, "Connecting…" banner, pagination loader, button busy state (spinner replaces icon, keeps width).
- `IBusyService`/`LoadingState` enum (`Idle, Loading, Refreshing, Error, Empty`) on every list ViewModel; Views bind via `DataTrigger`/`VisualStateManager`-like templates.
- Simulated latency in mock services (configurable) so every loading state is demonstrable.

### 6.9 Presence & realtime simulation
Mock `IRealtimeSimulator` that periodically: delivers incoming messages (text/photo/voice), toggles typing indicators, changes online status, advances read receipts — so the UI feels alive for demos.

---

## 7. CUSTOMIZATION SYSTEM (first-class feature)

Page/overlay **"Appearance"** with a **live preview** (mini chat rendered with real styles, updates instantly as controls change):

| Group | Controls |
|---|---|
| Theme | Dark / Light / Auto (follow Windows), preset cards (Aurora Night, Frost Day, Neon Tokyo, Forest Mist, Sunset Amber, Mono Ink, Custom) shown as 3D tilt cards |
| Accent | Swatches (12) + custom picker + gradient editor (2 stops, angle) |
| Glass | Blur radius (0–60), Tint opacity (0–40 %), Noise on/off, Border glow intensity, Reduce transparency |
| 3D / Depth | Depth intensity (0–100 → shadow & tilt multiplier), Parallax on/off, Tilt on/off |
| Background | Gradient presets, ambient orbs on/off + speed, custom image (with blur/dim sliders), solid color |
| Chat bubbles | Corner radius (8–24), Tail style (Telegram / Rounded / None), Bubble style (Glass / Solid / Gradient), Max width %, Spacing |
| Typography | Font family (Inter / Segoe UI Variable / Cascadia), message font size 12–20, line spacing |
| Layout | Density (Compact / Comfortable / Spacious), List item height, Show avatars in groups, Info panel default |
| Motion | Animation speed (0.5×–2×), Reduce motion, Enable sounds |
| Language | EN / FA (RTL switch) — string resources `Resources/Strings.<lang>.resx` |

Architecture: `ThemeService` + `IAppearanceSettings` (observable, immutable snapshot records for persistence). Changing a setting updates **dynamic resources** at runtime (swap `ResourceDictionary` / mutate token brushes through a `ThemeManager` — brushes referenced with `DynamicResource`). Support **Import/Export theme JSON**, "Reset to default", and per-chat wallpaper override (stretch).

---

## 8. ARCHITECTURE (CLEAN MVVM)

### 8.1 Solution layout

```
AuroraGlass.sln
├─ src/
│  ├─ AuroraGlass.App/                 (WPF startup, Host, DI composition root, App.xaml, Shell window)
│  ├─ AuroraGlass.Presentation/        (Views, ViewModels, Controls, Behaviors, Converters, Themes)
│  │   ├─ Views/        (ShellView, ChatListView, ChatView, ContactsView, AppearanceView, SettingsView, dialogs…)
│  │   ├─ ViewModels/   (ShellViewModel, ChatListViewModel, ChatViewModel, MessageViewModel*, ContactsViewModel…)
│  │   ├─ Controls/     (GlassPanel, OrbButton, Avatar3D, WaveformControl, MosaicPanel, …)
│  │   ├─ Behaviors/    (Tilt3D, ParallaxBehavior, AutoScroll, DragDropReorder, FocusOnLoad, TextBoxAutoGrow…)
│  │   ├─ Converters/
│  │   ├─ Themes/       (Tokens.xaml, Colors.Dark.xaml, Colors.Light.xaml, Brushes.xaml, Typography.xaml,
│  │   │                 Effects.xaml, Motion.xaml, Controls/*.xaml, Generic.xaml)
│  │   ├─ Resources/    (Fonts, Noise.png, Icons.xaml geometry dictionary, Strings.*.resx)
│  │   └─ Services/     (WpfDialogService, WpfNavigationService, ThemeManager, WindowBackdropService)
│  ├─ AuroraGlass.Core/                (pure .NET: Models, Abstractions, Messaging, Utilities — no WPF refs)
│  │   ├─ Models/       (Chat, Message, MessageContent*, Contact, User, Folder, MediaInfo, AppearanceSettings…)
│  │   ├─ Abstractions/ (IChatService, IMessageService, IContactService, IFolderService, ISettingsStore,
│  │   │                 IVoiceRecorder, IAudioPlaybackService, IMediaPlayerService, IImageCache, IClock…)
│  │   └─ Messaging/    (strongly-typed messenger events: MessageReceived, ChatUpdated, ThemeChanged…)
│  └─ AuroraGlass.Infrastructure/      (Mock services, JSON settings store, NAudio recorder, LibVLC player, simulator)
└─ tests/
   └─ AuroraGlass.Tests/               (ViewModel + service unit tests)
```

Dependency rule: `App → Presentation → Core ← Infrastructure`; Presentation never references Infrastructure types directly (only via DI-registered interfaces in App).

### 8.2 MVVM conventions
- ViewModels inherit `ObservableObject`/`ObservableRecipient`; use source generators, `AsyncRelayCommand` with cancellation & `IsRunning` binding.
- **ViewModel-first navigation**: `DataTemplate` per ViewModel in `DataTemplates.xaml`; `ContentControl` hosts with animated transitions (custom `TransitionPresenter`).
- `INavigationService.NavigateTo<TViewModel>(params)`; `IDialogService.ShowAsync<TDialogViewModel>()` returning results.
- Messaging via `WeakReferenceMessenger` for cross-VM events only (not for parent→child).
- Collections: `ObservableCollection<T>` + batch-update helper (`RangeObservableCollection`) to avoid N change notifications.
- Message VMs per content type (`TextMessageViewModel`, `PhotoMessageViewModel`, `VoiceMessageViewModel`, `VideoMessageViewModel`, `FileMessageViewModel`, `SystemMessageViewModel`) with a `DataTemplateSelector` or implicit DataTemplates.
- UI-thread safety via `IUiDispatcher` abstraction; no `Dispatcher.Invoke` in VMs.
- Validation: `ObservableValidator` + inline error display.
- Disposal: VMs that subscribe to events implement `IDisposable` / use weak events.

### 8.3 Styling architecture
- `Tokens.xaml` (spacing, radius, durations) → `Colors.*.xaml` (theme-specific) → `Brushes.xaml` (all `DynamicResource`) → `Controls/*.xaml` (implicit & keyed styles with `BasedOn`) → merged in `Generic.xaml`/`App.xaml`.
- Naming: `Brush.Glass.Tint`, `Style.Button.Primary`, `Anim.Fade.In`, `Geo.Icon.Send`.
- All animations as reusable `Storyboard` resources + attached behaviors; no duplicated inline storyboards.
- Freeze all static `Freezable`s; cache heavy `DrawingBrush`es; use `BitmapCache` (`RenderAtScale`) on blurred/glass layers that don't change; set `RenderOptions.BitmapScalingMode="HighQuality"` for avatars, `LowQuality` for fast scrolling thumbnails.

---

## 9. MOCK DATA REQUIREMENTS
- 40+ chats (mix of private/group/channel/bot/saved), 150+ contacts with generated avatars (gradient+initials) and a few real image assets.
- One "showcase" chat with 500+ messages containing every content type, long text, RTL text, links, reactions, replies, forwarded messages, albums, voice with generated waveforms, videos (sample files or placeholders), system messages ("Alice joined", date separators).
- Deterministic seed so screenshots are reproducible.

---

## 10. PERFORMANCE BUDGET
- Cold start to interactive shell: < 1.5 s on mid-range laptop.
- Scrolling 10k messages: ≥ 55 FPS; memory < 400 MB with 1k images loaded (LRU).
- Max 1 shared timer/storyboard for shimmer, 1 for ambient orbs; pause all when window minimized/inactive.
- Avoid nested `DropShadowEffect` on list items (use shared pre-rendered shadows or border tricks); disable blur on items while scrolling fast, re-enable on idle.
- Provide a **Performance mode** auto-switch (GPU tier detection via `RenderCapability.Tier`).

---

## 11. ERROR HANDLING & LOGGING
Global exception handlers (Dispatcher, AppDomain, TaskScheduler) → friendly glass error dialog + log; `Microsoft.Extensions.Logging` with file sink (`Serilog` optional). VM commands must not crash UI; surface failures through `IToastService`.

---

## 12. LOCALIZATION & RTL
String resources (en, fa); `FlowDirection` bound to `AppLanguage`; mirror layout (nav rail to right in RTL), bubbles mirror; Persian digits option; Jalali date option (stretch).

---

## 13. TESTING
Unit tests for: message grouping, date separators, folder filtering, unread counters, send state machine, recorder state machine (idle→recording→locked→sending/canceled), theme manager (token switching, JSON round-trip), mosaic layout algorithm, search. Include a few UI-less snapshot tests of ViewModels state.

---

## 14. DOCUMENTATION DELIVERABLES
- `README.md` (features, screenshots placeholders, how to run, architecture diagram in Mermaid).
- `docs/DESIGN_SYSTEM.md` (tokens, components, do/don't).
- `docs/ARCHITECTURE.md`.
- XML docs on all public types; concise comments only for non-obvious "why".

---

## 15. CODE QUALITY RULES
- Analyzers on, warnings as errors, `.editorconfig` (file-scoped namespaces, `_camelCase` private fields, `var` when obvious).
- One class per file; ≤ 300 lines per file where reasonable; small focused VMs (compose, don't create god-VMs).
- No magic numbers in XAML/C# — reference tokens.
- Zero binding errors in Output window (add a debug-time `BindingErrorTraceListener` that throws/logs).
- Accessibility & localization strings never hard-coded.

---

## 16. DELIVERY PHASES (commit after each)
1. **Foundation**: solution, DI host, token dictionaries (dark/light), `ThemeManager`, shell window with custom chrome + backdrop service, `GlassPanel`, ambient background.
2. **Controls**: component library (section 5) with a hidden "Gallery" page showing every control in all states.
3. **Chat list + folders + search** with mock data, skeletons, context menus, tab sliding indicator.
4. **Chat view**: virtualized messages, text bubbles, grouping, reply/edit/react/forward, composer, typing, scroll behaviors.
5. **Media**: photos (mosaic, viewer, upload progress), video (thumbnail, player), voice (recorder UI, waveform, playback).
6. **Contacts + profile + info panel**.
7. **Customization**: Appearance page with live preview, persistence, import/export.
8. **Polish**: motion pass, 3D tilt/parallax, splash, empty/error states, accessibility, RTL, performance pass, tests, docs.

---

## 17. ACCEPTANCE CHECKLIST
- [ ] Builds with 0 warnings; app launches to splash → shell.
- [ ] No code-behind logic; all interactions through commands/behaviors.
- [ ] Every control has all visual states and works with keyboard.
- [ ] Glass effect works on Windows 10 & 11 (fallbacks verified) and degrades gracefully.
- [ ] Theme switch (dark/light/preset/custom) updates the live UI instantly without restart.
- [ ] Text, photo (single + album), voice (record + play), video (thumb + viewer) messages fully demonstrable.
- [ ] Loading skeletons/spinners/progress rings visible in realistic flows.
- [ ] Folder tabs filter, reorder, badge correctly.
- [ ] Contacts: grouping, search, profile, add contact.
- [ ] 60 FPS scroll on 10k messages; memory within budget.
- [ ] RTL + Persian strings verified.
- [ ] Tests green; docs present.

---

## 18. EXTRA ITEMS YOU MAY HAVE FORGOTTEN (INCLUDE THEM)
- **Notifications**: in-app toast for new messages from other chats + optional Windows toast, unread badge on taskbar icon (overlay icon), tray icon with menu.
- **Drafts**, **pinned messages bar**, **scheduled/silent send** (placeholder), **message link preview cards** (title/desc/thumbnail).
- **Stickers/GIF panel** (placeholder tabs with emoji/sticker/GIF grid).
- **Context-aware window title** & **multi-window**: open chat in a separate window (stretch).
- **Lock screen / passcode** overlay with blurred glass pad (stretch).
- **Drag & drop files** anywhere in chat; **paste image** from clipboard; **save/copy image**.
- **Link/URL safe-open confirmation** dialog for external links.
- **Window snapping/maximize** behaviors with custom chrome (DPI-aware, multi-monitor, per-monitor DPI v2).
- **Sound effects** (send/receive/record) with on/off.
- **Easter-egg / delight**: confetti burst of glass shards on 🎉 reaction, elastic "pop" on new badge.
- **Telemetry-free**, **crash-safe settings writes** (atomic file replace).
- **Dev tools**: hidden debug panel (FPS counter, binding errors, toggle mock latency, trigger simulator events).

---

## 19. HOW TO WORK
1. Start by proposing the exact folder/file plan and the technical approach for the glass backdrop and the 3D tilt (short, with trade-offs), then implement phase by phase.
2. After each phase: build, run, fix warnings, briefly summarize what changed and what's next.
3. If a requirement is ambiguous, choose the most polished option, document the decision in `docs/DECISIONS.md`, and continue — do not stop to ask unless blocked.
4. Prioritize **visual excellence, smoothness, and clean MVVM** above feature count; every pixel, easing curve and state should feel intentional.
