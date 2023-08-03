```mermaid
stateDiagram
[*]-->idle
idle-->initialized:saisyohaGu %% 最初はグー
initialized --> showdown: hand selected %% 手を選択
showdown-->win:you win
showdown-->lose:you lose
showdown-->draw:It is a draw
win-->end:Lapse of a certain time
lose-->end:Lapse of a certain time
draw-->end:Lapse of a certain time
```