using System.Media;

namespace BattleshipGame
{
    public static class SoundManager
    {
        private static readonly SoundPlayer? hitSound;
        private static readonly SoundPlayer? missSound;
        private static readonly SoundPlayer? sinkSound;
        private static readonly SoundPlayer? victorySound;        static SoundManager()
        {
            try
            {
                string systemRoot = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                string mediaPath = Path.Combine(systemRoot, "Media");

                // Using Windows system sounds as they're readily available
                hitSound = new SoundPlayer(Path.Combine(mediaPath, "Windows Notify.wav"));
                missSound = new SoundPlayer(Path.Combine(mediaPath, "Windows Navigation Start.wav"));
                sinkSound = new SoundPlayer(Path.Combine(mediaPath, "Windows Exclamation.wav"));
                victorySound = new SoundPlayer(Path.Combine(mediaPath, "tada.wav"));

                // Pre-load sounds
                hitSound.LoadAsync();
                missSound.LoadAsync();
                sinkSound.LoadAsync();
                victorySound.LoadAsync();
            }
            catch
            {
                // Fail silently if sounds can't be loaded
            }
        }        public static void PlayHit()
        {
            try
            {
                hitSound?.Play();
            }
            catch (System.IO.FileNotFoundException)
            {
                // Silently fail if sound file is not found
            }
        }

        public static void PlayMiss()
        {
            try
            {
                missSound?.Play();
            }
            catch (System.IO.FileNotFoundException)
            {
                // Silently fail if sound file is not found
            }
        }

        public static void PlaySink()
        {
            try
            {
                sinkSound?.Play();
            }
            catch (System.IO.FileNotFoundException)
            {
                // Silently fail if sound file is not found
            }
        }

        public static void PlayVictory()
        {
            try
            {
                victorySound?.Play();
            }
            catch (System.IO.FileNotFoundException)
            {
                // Silently fail if sound file is not found
            }
        }
    }
}
