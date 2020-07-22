namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class Constants
    {
        public const string LONG_RECOGNIZE_REQUEST_URL = "https://speech.googleapis.com/v1/speech:longrunningrecognize";
        public const string RECOGNIZE_REQUEST_URL = "https://speech.googleapis.com/v1/speech:recognize";
        public const string STREAMING_RECOGNIZE_REQUEST_URL = "https://speech.googleapis.com/v1/speech:recognize";
        public const string API_KEY_PARAM = "?key=";


        public const string GC_API_KEY = "AIzaSyCj_n9FN1JGQAiz5tGfY3-QTiv4TplePXs"; // Google Cloud API Key. Only for test! Use your own API Key in Live!


        public const double AUDIO_DETECT_RATIO = 32768.0;
    }
}