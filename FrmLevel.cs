using Fall2020_CSC403_Project.code;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using Fall2020_CSC403_Project.Properties;
using System.Diagnostics;

namespace Fall2020_CSC403_Project
{
    public partial class FrmLevel : Form
    {
        /// <summary>
        /// private Fields
        /// </summary>
        private Player player;

        private Enemy enemyPoisonPacket;
        private Enemy bossKoolaid;
        private Enemy enemyCheeto;
        private Enemy enemyBoy;
        private Enemy enemyDevil;
        private Character[] walls;

        private DateTime timeBegin;
        private FrmBattle frmBattle;
        private SoundPlayer backgroundMusic;
        private SoundPlayer fight;
        private Form childForm;
        private Label lblRating;
        private TextBox txtRating;
        private Button btnSubmitRating;

        /// <summary>
        /// Constructor 
        /// </summary>
        public FrmLevel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLevel_Load(object sender, EventArgs e)
        {
            //For Background Music
            backgroundMusic = new  SoundPlayer(Resources.backgroundMusic);
            backgroundMusic.PlayLooping();
            const int PADDING = 7;
            const int NUM_WALLS = 13;

            player = new Player(CreatePosition(picPlayer), CreateCollider(picPlayer, PADDING));
            bossKoolaid = new Enemy(CreatePosition(picBossKoolAid), CreateCollider(picBossKoolAid, PADDING));
            enemyPoisonPacket = new Enemy(CreatePosition(picEnemyPoisonPacket), CreateCollider(picEnemyPoisonPacket, PADDING));
            enemyCheeto = new Enemy(CreatePosition(picEnemyCheeto), CreateCollider(picEnemyCheeto, PADDING));
            // To add new enimies
            enemyBoy = new Enemy(CreatePosition(picEnemyBoy), CreateCollider(picEnemyBoy, PADDING));
            enemyDevil = new Enemy(CreatePosition(picEnemyDevil), CreateCollider(picEnemyDevil, PADDING));

            bossKoolaid.Img = picBossKoolAid.BackgroundImage;
            enemyPoisonPacket.Img = picEnemyPoisonPacket.BackgroundImage;
            enemyCheeto.Img = picEnemyCheeto.BackgroundImage;
            enemyBoy.Img = picEnemyBoy.BackgroundImage;
            enemyDevil.Img = picEnemyDevil.BackgroundImage;

            bossKoolaid.Name = "BossKoolAid";
            enemyPoisonPacket.Name = "PoisonPacket";
            enemyCheeto.Name = "Cheeto";
            enemyBoy.Name = "Boy";
            enemyDevil.Name = "Devil";

            bossKoolaid.Color = Color.Red;
            enemyPoisonPacket.Color = Color.Green;
            enemyCheeto.Color = Color.FromArgb(255, 245, 161);
            enemyBoy.Color = Color.Aqua;
            enemyDevil.Color = Color.LightPink;

            walls = new Character[NUM_WALLS];
            for (int w = 0; w < NUM_WALLS; w++)
            {
                PictureBox pic = Controls.Find("picWall" + w.ToString(), true)[0] as PictureBox;
                walls[w] = new Character(CreatePosition(pic), CreateCollider(pic, PADDING));
            }

            Game.player = player;
            timeBegin = DateTime.Now;
        }

        private Vector2 CreatePosition(PictureBox pic)
        {
            return new Vector2(pic.Location.X, pic.Location.Y);
        }

        private Collider CreateCollider(PictureBox pic, int padding)
        {
            Rectangle rect = new Rectangle(pic.Location, new Size(pic.Size.Width - padding, pic.Size.Height - padding));
            return new Collider(rect);
        }

        private void FrmLevel_KeyUp(object sender, KeyEventArgs e)
        {
            player.ResetMoveSpeed();
        }

        private void tmrUpdateInGameTime_Tick(object sender, EventArgs e)
        {
            TimeSpan span = DateTime.Now - timeBegin;
            string time = span.ToString(@"hh\:mm\:ss");
            lblInGameTime.Text = "Time: " + time.ToString();
        }

        private void tmrPlayerMove_Tick(object sender, EventArgs e)
        {
            // move player
            player.Move();

            // check collision with walls
            if (HitAWall(player))
            {
                player.MoveBack();
            }

            // check collision with enemies
            if (HitAChar(player, enemyPoisonPacket))
            {
                // to check enemy is alive

                if (enemyPoisonPacket.Health > 0)
                {
                   // to fight Enemy 
                    Fight(enemyPoisonPacket);
                }
                if (enemyPoisonPacket.Health < 0)
                {
                    // to clear dead bodies
                    picEnemyPoisonPacket.Hide();
                } 
            }
            else if (HitAChar(player, enemyCheeto))
            {

                if (enemyCheeto.Health > 0)
                {

                    Fight(enemyCheeto);
                }
                if (enemyCheeto.Health < 0)
                {
                    picEnemyCheeto.Hide();
                }
            }
            else if (HitAChar(player, enemyBoy))
            {

                if (enemyBoy.Health > 0)
                {

                    Fight(enemyBoy);
                }
                if (enemyBoy.Health < 0)
                {
                    picEnemyBoy.Hide();
                }
            }
            else if (HitAChar(player, enemyDevil))
            {

                if (enemyDevil.Health > 0)
                {

                    Fight(enemyDevil);
                }
                if (enemyDevil.Health < 0)
                {
                    picEnemyDevil.Hide();
                }
            }
            if (HitAChar(player, bossKoolaid))
            {

                if (bossKoolaid.Health > 0)
                {   
                    player.RechageHealth();
                    Fight(bossKoolaid);
                }

                if (bossKoolaid.Health < 0)
                {
                    picBossKoolAid.Hide();
                }
            }

            // update player's picture box
            picPlayer.Location = new Point((int)player.Position.x, (int)player.Position.y);
        }

        private bool HitAWall(Character c)
        {
            bool hitAWall = false;
            for (int w = 0; w < walls.Length; w++)
            {
                if (c.Collider.Intersects(walls[w].Collider))
                {
                    hitAWall = true;
                    break;
                }
            }
            return hitAWall;
        }

        private bool HitAChar(Character you, Character other)
        {
            return you.Collider.Intersects(other.Collider);
        }

        private void Fight(Enemy enemy)
        {

            // to add sound effects on collision with enymy
            fight = new SoundPlayer(Resources.fight);
            fight.Play();

            player.ResetMoveSpeed();
            player.MoveBack();
            // get the FrmBattle Instance
            frmBattle = FrmBattle.GetInstance(enemy);
            // to show the FrmBattle Instance
            frmBattle.Show();


            if (enemy == bossKoolaid)
            {
                frmBattle.SetupForBossBattle();
            }
        }

        private void FrmLevel_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Left:
                    player.GoLeft();
                    break;

                case Keys.Right:
                    player.GoRight();
                    break;

                case Keys.Up:
                    player.GoUp();
                    break;

                case Keys.Down:
                    player.GoDown();
                    break;

                default:
                    player.ResetMoveSpeed();
                    break;
            }
        }

        private void lblInGameTime_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// to bring the rating form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picReviewUs_Click(object sender, EventArgs e)
        {
            //initialize a child form
            childForm = new Form(); 
            childForm.Width =  500;
            childForm.Height = 500;
            //initialize a label
            lblRating = new Label();
            lblRating.AutoSize = true;
            lblRating.Text = "Please Rate Us from 1 to 5";
            lblRating.Location = new Point(100, 230);
            //initialize a Textbox
            txtRating = new TextBox();
            txtRating.Location = new Point(240, 230);
            //initialize a button
            btnSubmitRating = new Button();
            btnSubmitRating.Text = "Submit";
            btnSubmitRating.BackColor = Color.Orange;
            btnSubmitRating.Click += btnSubmitRating_Click;
            btnSubmitRating.Location = new Point(370, 230);
            //add controls to child Form
            childForm.Controls.Add(lblRating);
            childForm.Controls.Add(txtRating);
            childForm.Controls.Add(btnSubmitRating);
            childForm.StartPosition = FormStartPosition.CenterScreen;
            //finally display it
            childForm.Show(this); 
        }
        /// <summary>
        /// to submit the rating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSubmitRating_Click(object sender, EventArgs e)
        {
            // to validate the user's input
            int input = 0;
            bool isNum = Int32.TryParse(txtRating.Text, out input);

            if (!isNum || input < 1|| input > 5)
            {
                txtRating.Text = "";
                MessageBox.Show("Please Enter value only from 1 to 5");

            }
            else
            {
                childForm.Close();
            }

            
        }
        /// <summary>
        /// to restart the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picRestartGame_Click(object sender, EventArgs e)
        {

            Application.Restart();
        }
    }
}
