using System.Media;

namespace BattleshipGame
{
    public static class SoundManager
    {
        private static readonly SoundPlayer? hitSound;
        private static readonly SoundPlayer? missSound;
        private static readonly SoundPlayer? sinkSound;
        private static readonly SoundPlayer? victorySound;

        static SoundManager()
        {
            try
            {
                // Using Windows system sounds as they're readily available
                hitSound = new SoundPlayer(@"%SystemRoot%\Media\Windows Notify.wav");
                missSound = new SoundPlayer(@"%SystemRoot%\Media\Windows Navigation Start.wav");
                sinkSound = new SoundPlayer(@"%SystemRoot%\Media\Windows Exclamation.wav");
                victorySound = new SoundPlayer(@"%SystemRoot%\Media\tada.wav");

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
        }

        public static void PlayHit()
        {
            hitSound?.Play();
        }

        public static void PlayMiss()
        {
            missSound?.Play();
        }

        public static void PlaySink()
        {
            sinkSound?.Play();
        }

        public static void PlayVictory()
        {
            victorySound?.Play();
        }
    }
}
