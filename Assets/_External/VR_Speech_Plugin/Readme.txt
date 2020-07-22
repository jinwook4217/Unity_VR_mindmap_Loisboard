
ReadMe.txt

—————————————————————————————

Version History:
 1.0 : First release. Speech Recognition API run Android Plugin.
 2.0 : Bug-fix, Speech Recognition API run Service of UiThread safe.

バージョン履歴：
 1.0:Android Pluginで音声認識機能を実装
 2.0:動作不良のバグ修正。音声認識部分をPluginをServiceで稼働、UiThreadで動作するように調整。

—————————————————————————————

Programing Document - Readme.pdf

Demo Scene - Speech_plugin.unity
C# Script - Speech_plugin.cs
Android Manifest file - AndroidManifest.xml
Android Plugin jar file - speech_lib2.jar

—————————————————————————————

Demo Scene Sound Data for jeremysykes.
Special Thanks. :)

jump.wav & powerup.wav 
License : CC0

—————————————————————————————

This Plugin Android Native Extension.
Speech Recognition Function add Plugin.

これのプラグインはAndroidのネイティブ拡張です。
音声認識機能を追加することができます。

This Document(PDF) Android Plugin injection Step by Step.

このドキュメント(PDF)のステップバイステップで進めてください。

—————————————————————————————

(Step1)
./Assets/Plugins/Android.manifest

Android.manifest file add write permission & service.

Androidマニュフェストファイルに下記のパーミッションとサービスを追加してください。

<service android:enabled="true" android:name="com.speech_recognition_v2.Speech_lib" />

<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />

if You don’t Android.manifest file,
It’s add this Asset in Android.manifest file and Create Folder.  ファイルが存在しない場合はフォルダを作成し、 このAsset内のAndroid.manifestを追加してください。

(Step2)
./Assets/Plugins/Android

“speech_lib2” jar file add This folder.
“speech_lib2”jarファイルを上記のフォルダに追加して下さい。

(Step3)
./Assets
Add C# script “Speech_plugin”.
“Callback” method and “Error CallBack” method.
It’s add “Add Component” Game Object.

./Assets
フォルダに“Speech_plugin” C#スクリプトを追加して下さい。
この中の“Callback”と“Error CallBack”に音声認識の文字列やエラーが返信されます。
この“Speech_plugin”を自分の設定したいGame Objectに“Add Component”で追加してください。

(Step4)
Android native Speech Recognition function return is Resulting Error.
it’s you keyword to correct.
Hello Resulting English or Japanese, Please Android-Logcat test. 

Androidネイティブ音声認識機能からAndroid Logcatで返信されてくるメッセージを確認しながら
キーワードの修正を行ってください。同じハローの場合でも英語だったり日本語だったりします。

—————————————————————————————
Read Thank You!
