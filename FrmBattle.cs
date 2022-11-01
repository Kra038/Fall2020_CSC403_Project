using Fall2020_CSC403_Project.code;
using Fall2020_CSC403_Project.Properties;
using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Fall2020_CSC403_Project
{
    public partial class FrmBattle : Form
    {
        public static FrmBattle instance = null;
        private Enemy enemy;
        private Player player;
        private SoundPlayer punch;
        private SoundPlayer backgroundMusic;
        private PictureBox picGameOver;
        private PictureBox picRestart;
        private PictureBox picYouLose;

        private FrmBattle()
        {
            InitializeComponent();
            player = Game.player;
        }

        public void Setup()
        {
            // update for this enemy
            picEnemy.BackgroundImage = enemy.Img;
            picEnemy.Refresh();
            BackColor = enemy.Color;
            picBossBattle.Visible = false;

            // Observer pattern
            enemy.AttackEvent += PlayerDamage;
            player.AttackEvent += EnemyDamage;

            // show health
            UpdateHealthBars();
        }

        public void SetupForBossBattle()
        {
            picBossBattle.Location = Point.Empty;
            picBossBattle.Size = ClientSize;
            picBossBattle.Visible = true;

            SoundPlayer simpleSound = new SoundPlayer(Resources.final_battle);
            simpleSound.Play();

            tmrFinalBattle.Enabled = true;
        }

        public static FrmBattle GetInstance(Enemy enemy)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new FrmBattle();
                instance.enemy = enemy;
                instance.Setup();
            }
            return instance;
        }

        private void UpdateHealthBars()
        {
            float playerHealthPer = player.Health / (float)player.MaxHealth;
            float enemyHealthPer = enemy.Health / (float)enemy.MaxHealth;

            const int MAX_HEALTHBAR_WIDTH = 226;
            lblPlayerHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * playerHealthPer);
            lblEnemyHealthFull.Width = (int)(MAX_HEALTHBAR_WIDTH * enemyHealthPer);

            lblPlayerHealthFull.Text = player.Health.ToString();
            lblEnemyHealthFull.Text = enemy.Health.ToString();
        }

        private void btnAttack_Click(object sender, EventArgs e)
        {

            // to add sound effects on Attack
            punch = new SoundPlayer(Resources.punch);
            punch.Play();
            player.OnAttack(-4);
            if (enemy.Health > 0)
            {
                enemy.OnAttack(-2);
            }
            // to update the healthbars
            UpdateHealthBars();
            if (enemy.Health <= 0)
            {
                // to close frmBattle once Enymy is dead
                instance = null;
                Close();

                // to show player has won the battle
                if (enemy.Name == "BossKoolAid")
                {
                    FrmLevel.ActiveForm.Controls.Clear();
                    backgroundMusic = new SoundPlayer(Resources.backgroundMusic);
                    backgroundMusic.PlayLooping();

                    picYouLose = new PictureBox();
                    picYouLose.BackgroundImage = Resources.you_win;
                    picYouLose.BackgroundImageLayout = ImageLayout.Stretch;
                    int pointX = (FrmLevel.ActiveForm.Width / 2) - (picYouLose.Width / 2);
                    int pointY = (FrmLevel.ActiveForm.Height / 2) - (picYouLose.Height / 2);
                    picYouLose.Location = new Point(pointX - 200, pointY);
                    FrmLevel.ActiveForm.Controls.Add(picYouLose);
                    picRestart = new PictureBox();
                    picRestart.Name = "picRestart";
                    picRestart.BackgroundImage = Resources.restart;
                    picRestart.BackgroundImageLayout = ImageLayout.Stretch;
                    picRestart.Location = new Point(pointX + 150, pointY);
                    picRestart.Click += picRestart_Clicked;
                    FrmLevel.ActiveForm.Controls.Add(picRestart);
                }
                
              

            }
            if (player.Health <= 0)
            {
                // to close frmBattle once player is dead
                instance = null;
                Close();
                FrmLevel.ActiveForm.Controls.Clear();
                // to show player has lost the battle
                picGameOver = new PictureBox();
                picGameOver.BackgroundImage = Resources.game_over;
                picGameOver.BackgroundImageLayout = ImageLayout.Stretch;
                int pointX = (FrmLevel.ActiveForm.Width / 2) - (picGameOver.Width / 2);
                int pointY = (FrmLevel.ActiveForm.Height / 2) - (picGameOver.Height / 2);
                picGameOver.Location = new Point(pointX,pointY );
                FrmLevel.ActiveForm.Controls.Add(picGameOver);
                picYouLose = new PictureBox();
                picYouLose.BackgroundImage = Resources.you_lose;
                picYouLose.BackgroundImageLayout = ImageLayout.Stretch;
                picYouLose.Location = new Point(pointX-200,pointY);
                FrmLevel.ActiveForm.Controls.Add(picYouLose);
                picRestart = new PictureBox();
                picRestart.Name = "picRestart";
                picRestart.BackgroundImage = Resources.restart;
                picRestart.BackgroundImageLayout = ImageLayout.Stretch;
                picRestart.Location = new Point(pointX+150, pointY);
                picRestart.Click += picRestart_Clicked;
                FrmLevel.ActiveForm.Controls.Add(picRestart);
            }
        }

        private void EnemyDamage(int amount)
        {
            enemy.AlterHealth(amount);
        }

        private void PlayerDamage(int amount)
        {
            player.AlterHealth(amount);
        }

        private void tmrFinalBattle_Tick(object sender, EventArgs e)
        {
            picBossBattle.Visible = false;
            tmrFinalBattle.Enabled = false;
        }

        public void picRestart_Clicked(object sender, EventArgs e)
        {

            Application.Restart();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            // to cancel the current battle 
            this.Close();
        }
    }
}
