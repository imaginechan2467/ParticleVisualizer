# ParticleVisualizer
パーティクルエフェクトを確認するための簡易ツールです。

======操作方法======

1:確認したいエフェクトのフォルダをクリックして、インスペクタの上の方のAddressableのチェックを付ける。

2:シーン内のEffectVisualizerオブジェクトに付いているSC_EffectVisualizerをインスペクタから設定する。
2-1:確認したいエフェクト、もしくはエフェクトの入っているフォルダのパスを入力。
例：Asset/Plugins/Polygon Arsenal/prefabs/Combat/***

3-1:TypeをIn Gameにすると、インスペクタで入力されているパスのエフェクトゲーム画面でキーボード操作で一個一個のエフェクトを再生したり移動したりして確認できます。
・カメラやエフェクトの移動速度が遅ければ、インスペクタの速度変数を調整して下さい。
・エフェクトによっては以下のエフェクト移動操作でも移動しないものがあります。
====In Gameモードでの操作====
Shift押してる時
WASDQE : エフェクト移動
Shift押して無い時
WASDQE : カメラ移動
P      : 確認終了(ゲーム終了)
←→     : エフェクト切り替え
Space  : エフェクト再生

3-2:TypeをPictureにすると、ゲーム開始時に入力されているパスの中(子供フォルダの中身を含める)のエフェクトを全て自動で再生して、再生時間の中間(2秒の物なら1秒)で写真を取り、Asset/Screenshotsフォルダ内に保存します。確認して要らなければ消してOKです。

3-3:TypeをMovieかMaxにすると何も無く終了します。未実装です。ただ、動画を撮影したい場合は、Recorderプラグインを入れてあるので、ワンクリックでゲーム画面を撮影できます。

