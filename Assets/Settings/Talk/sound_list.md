# サウンドリスト

## BGM 一覧

| 名前 | 雰囲気・イメージ | 推奨秒数 | 使用ファイル |
|---|---|---|---|
| BGM_Bar | ジャジーでけだるいサイバーパンク・ローファイ。バーのネオンと合成酒の雰囲気 | 60〜90秒ループ | intro.txt |
| BGM_Casino | テンションのある電子音楽。カジノの緊張感と高揚感 | 60〜90秒ループ | phase_1.txt, phase_2.txt, phase_3.txt |
| BGM_Crisis | 切迫感のあるシンセ。正体バレ・窮地の緊張 | 45〜60秒ループ | phase_3.txt（お面剥ぎ取り後に切替） |
| BGM_Victory | 高揚感のある明るい電子音楽。勝利の達成感 | 30〜60秒ループ | gameclear_1.txt, gameclear_2.txt, gameclear_3.txt, gameover_4.txt（前半）, outro.txt（前半） |
| BGM_Despair | 物悲しいアンビエント。敗北と後悔、ネオンの冷たさ | 60〜90秒ループ | gameover_1.txt, gameover_2.txt, gameover_3.txt, gameover_4.txt（後半切替） |
| BGM_Backdoor | 疾走感のあるエレクトロ。ハッキング・潜入の緊迫感 | 60〜90秒ループ | backdoor_1.txt, backdoor_2.txt |
| BGM_Ending | 温かくエモーショナルな曲。二人の友情、ハッピーエンド | 60〜90秒ループ | outro.txt（バーシーン切替後） |

## 効果音 一覧

| 名前 | どのような音か | 推奨秒数 | 使用ファイル |
|---|---|---|---|
| Footstep | 足音・駆け出す音。硬い床に響く靴音 | 1〜2秒 | intro.txt, phase_3.txt, backdoor_1.txt, backdoor_2.txt |
| Impact | ドラマチックな衝撃音。転換点やインパクトのある場面に | 0.5〜1秒 | intro.txt, gameclear_1.txt, gameclear_2.txt, gameclear_3.txt, gameover_4.txt, phase_3.txt（2箇所）, outro.txt |
| LensActivate | デジタルなAR起動音。電子的なチャイム感 | 1〜2秒 | phase_1.txt |
| Typing | キーボード・端末操作音。高速タイピング | 1〜2秒 | phase_1.txt, backdoor_1.txt, backdoor_2.txt（2箇所） |
| Decision | 決意の瞬間。短く力強いアクセント音 | 0.5〜1秒 | phase_1.txt, phase_2.txt, gameclear_1.txt, backdoor_success.txt |
| Alert | 警告・通信割り込み音。電子的なアラート | 1秒 | phase_2.txt, gameclear_2.txt, backdoor_1.txt |
| MaskOff | お面が剥がされる音。硬い素材が外れる音 | 0.5〜1秒 | phase_3.txt |
| Surprise | 驚きのリアクション。短い衝撃的な音 | 0.5秒 | phase_3.txt, gameover_4.txt, backdoor_2.txt, outro.txt |
| Jamming | 電波妨害・ノイズ。電子機器が停止する不快な音 | 1〜2秒 | phase_3.txt |
| Whisper | 小さな囁き。息遣い混じりの静かな音 | 1秒 | phase_3.txt, backdoor_success.txt |
| DoorUnlock | 電子錠解除。重い金属音＋電子ロック解除音 | 1〜2秒 | backdoor_1.txt |

## BGM 逆引き（ファイル → BGM）

| ファイル | BGM | 備考 |
|---|---|---|
| intro.txt | BGM_Bar | 冒頭から |
| phase_1.txt | BGM_Casino | 冒頭から |
| gameclear_1.txt | BGM_Victory | 冒頭から |
| gameover_1.txt | BGM_Despair | 冒頭から |
| phase_2.txt | BGM_Casino | 冒頭から |
| gameclear_2.txt | BGM_Victory | 冒頭から |
| gameover_2.txt | BGM_Despair | 冒頭から |
| phase_3.txt | BGM_Casino → BGM_Crisis | お面剥ぎ取り後に切替 |
| gameclear_3.txt | BGM_Victory | 冒頭から |
| gameover_3.txt | BGM_Despair | 冒頭から |
| gameover_4.txt | BGM_Victory → BGM_Despair | バーシーンで切替（勝利→後悔） |
| backdoor_1.txt | BGM_Backdoor | 冒頭から |
| backdoor_2.txt | BGM_Backdoor | 冒頭から |
| backdoor_success.txt | （BGM変更なし） | 直前のBGM継続 |
| outro.txt | BGM_Victory → BGM_Ending | バーシーンで切替 |
| twitter.txt | （BGM変更なし） | 直前のBGM継続 |

## 素材ファイル対応表（Assets/Sounds/）

各サウンドにつき候補を2つずつ用意。試聴して選定後、採用する方を本番ファイル名にリネームしてください。

### BGM

| サウンド名 | 候補 | ファイル名 | 元素材名 | サイト |
|---|---|---|---|---|
| BGM_Bar | 候補1 | BGM_Bar_1.mp3 | Jazz Background Music (Bar Restaurant Casino Mafia Whiskey) — BackgroundMusicForVideos | Pixabay |
| BGM_Bar | 候補2 | BGM_Bar_2.mp3 | The Best Jazz Club In New Orleans — PaoloArgento | Pixabay |
| BGM_Casino | 候補1 | BGM_Casino_1.mp3 | Cyberpunk Futuristic City Music — lNPLUSMUSIC | Pixabay |
| BGM_Casino | 候補2 | BGM_Casino_2.mp3 | Cyberpunk Metaverse Event Background Music — HitsLab | Pixabay |
| BGM_Crisis | 候補1 | BGM_Crisis_1.mp3 | Suspense Cyberpunk — The_Mountain | Pixabay |
| BGM_Crisis | 候補2 | BGM_Crisis_2.mp3 | Cyberpunk Techno — humanstudioedm | Pixabay |
| BGM_Victory | 候補1 | BGM_Victory_1.mp3 | Cyberpunk Futuristic Background — Tunetank | Pixabay |
| BGM_Victory | 候補2 | BGM_Victory_2.mp3 | Brain Implant (Cyberpunk Sci-Fi Trailer Action Intro) — VasilYatsevich | Pixabay |
| BGM_Despair | 候補1 | BGM_Despair_1.mp3 | Dark Sad Ambient Piano — Ashot_Danielyan | Pixabay |
| BGM_Despair | 候補2 | BGM_Despair_2.mp3 | Incurable - sad ambient cinematic track — BirdsDieAlone | Pixabay |
| BGM_Backdoor | 候補1 | BGM_Backdoor_1.mp3 | Hacking the System Loop — Ebunny | Pixabay |
| BGM_Backdoor | 候補2 | BGM_Backdoor_2.mp3 | No Cyber Future (Action Cyber Game) — Amaksi | Pixabay |
| BGM_Ending | 候補1 | BGM_Ending_1.mp3 | Embrace — Evgeny_Bardyuzha | Pixabay |
| BGM_Ending | 候補2 | BGM_Ending_2.mp3 | EONA - Emotional Ambient Pop — Rockot | Pixabay |

### 効果音

| サウンド名 | 候補 | ファイル名 | 元素材名 | サイト |
|---|---|---|---|---|
| Footstep | 候補1 | Footstep_1.mp3 | Footsteps on hard floor — OxidVideos | Pixabay |
| Footstep | 候補2 | Footstep_2.mp3 | concrete footsteps 1 — freesound_community | Pixabay |
| Impact | 候補1 | Impact_1.mp3 | Impact – Cinematic Boom — Universfield | Pixabay |
| Impact | 候補2 | Impact_2.mp3 | Cinematic Impact Hit — Universfield | Pixabay |
| LensActivate | 候補1 | LensActivate_1.mp3 | Sci-fi Technology Scanner — daviddumaisaudio | Pixabay |
| LensActivate | 候補2 | LensActivate_2.mp3 | Futuristic UI Beeps — soundzee | Pixabay |
| Typing | 候補1 | Typing_1.mp3 | Keyboard Typing Fast — VirtualZero | Pixabay |
| Typing | 候補2 | Typing_2.mp3 | Fast typing keyboard — Ategbe1 | Pixabay |
| Decision | 候補1 | Decision_1.mp3 | dramatic sting — u_903n3qx7rq | Pixabay |
| Decision | 候補2 | Decision_2.mp3 | Stab F# 01 — BRVHRTZ | Pixabay |
| Alert | 候補1 | Alert_1.mp3 | Extraterrestrial Alert Sound — FNX_Sound | Pixabay |
| Alert | 候補2 | Alert_2.mp3 | notification alert — (Generic Synth Alert) | Pixabay |
| MaskOff | 候補1 | MaskOff_1.mp3 | snap sound effect — rodrieffects | Pixabay |
| MaskOff | 候補2 | MaskOff_2.mp3 | bone snap — saboteurcomics | Pixabay |
| Surprise | 候補1 | Surprise_1.mp3 | Gasp — Universfield | Pixabay |
| Surprise | 候補2 | Surprise_2.mp3 | Sudden Shock Reveal Bass — kalsstockmedia | Pixabay |
| Jamming | 候補1 | Jamming_1.mp3 | Radio Frequency Interference — freesound_community | Pixabay |
| Jamming | 候補2 | Jamming_2.mp3 | Glitch transition SFX — khemrajdotin | Pixabay |
| Whisper | 候補1 | Whisper_1.mp3 | Eerie Space Whisper of the Underworld — FNX_Sound | Pixabay |
| Whisper | 候補2 | Whisper_2.mp3 | Whisper Voices 1 — helamangile | Pixabay |
| DoorUnlock | 候補1 | DoorUnlock_1.mp3 | Opening Metal Door — Universfield | Pixabay |
| DoorUnlock | 候補2 | DoorUnlock_2.mp3 | Electronic Lock Opening — freesound_community | Pixabay |
