using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpaceInvaders;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
	// go left and right boolean are set to false
	private bool _goLeft, _goRight;
	// this list items to remove will be used as a garbage collector
	private readonly List<Rectangle> _itemstoremove = [];
	// this int enemy images will help us change enemy pictures
	private int _enemyImages;
	// this is the enemy bullet timer
	private int _bulletTimer;
	// this is the enemy bullet timer limit and frequency
	private const int BulletTimerLimit = 90;

	// save the total number of enemies
	private int _totalEnemeis;
	// make a new instance of the dispatch timer class
	private readonly DispatcherTimer _dispatcherTimer = new();
	// image brush class that we will use as the player image called player skin
	private readonly ImageBrush _playerSkin = new();
	// the default enemy speed
	private int _enemySpeed = 6;

	private List<ImageSource> _invadersList;

	public MainWindow()
	{
		InitializeComponent();
		// set up the timer and events
		// link the dispatcher timer to a event called game engine
		_dispatcherTimer.Tick += GameEngine;
		// this timer will run every 20 milliseconds
		_dispatcherTimer.Interval = TimeSpan.FromMilliseconds(20);
		// start the timer
		_dispatcherTimer.Start();
		// load the player images from the images folder
		_playerSkin.ImageSource = GetImageSource(Properties.Resources.player);
		// assign the new player skin to the rectangle
		player1.Fill = _playerSkin;
		// run the make enemies function and tell it to make 30 enemies
		_invadersList = new List<ImageSource>
		{
			GetImageSource(Properties.Resources.invader1), GetImageSource(Properties.Resources.invader2),
			GetImageSource(Properties.Resources.invader3), GetImageSource(Properties.Resources.invader4),
			GetImageSource(Properties.Resources.invader5),
			GetImageSource(Properties.Resources.invader6), GetImageSource(Properties.Resources.invader7),
			GetImageSource(Properties.Resources.invader8)
		};
		MakeEnemies(30);

		
	}

	private void Canvas_KeyisDown(object sender, KeyEventArgs e)
	{
		switch (e.Key)
		{
			case Key.Left:
				_goLeft = true;
				break;
			case Key.Right:
				_goRight = true;
				break;
		}
	}

	private void Canvas_KeyIsUp(object sender, KeyEventArgs e)
	{
		switch (e.Key)
		{
			// this is the key up event
			// if the left key is let go set go left to false
			// if the right key is let go set go right to false
			case Key.Left:
				_goLeft = false;
				break;
			case Key.Right:
				_goRight = false;
				break;
			// below you have the if statement that will make the bullets
			// check if the space key is let go
			case Key.Space:
			{
				// clear all the items from the items to remove list first
				_itemstoremove.Clear();
				// make a new rectangle called new bullet and add a tag called bullet, height 20 width 5 backgroubnd white and border to red
				var newBullet = new Rectangle
				{
					Tag = "bullet",
					Height = 20,
					Width = 5,
					Fill = Brushes.White,
					Stroke = Brushes.Red
				};
				// place the bullet where the player is
				Canvas.SetTop(newBullet, Canvas.GetTop(player1) - newBullet.Height);
				Canvas.SetLeft(newBullet, Canvas.GetLeft(player1) + player1.Width / 2);
				// add the bullet to the screen
				myCanvas.Children.Add(newBullet);
				break;
			}
		}
	}
	private void EnemyBulletMaker(double x, double y)
	{ // this function creates the enemy bullets firing towards the player object in the game
		// see this function is passing through 2 variables x and y these will be location where we place the bullets
		// first create a new rectangle
		// this rectangle will have a tag called enemy bullet, height 40 pixels, width 15 pixels, background yellow border black and border size 5
		var newEnemyBullet = new Rectangle
		{
			Tag = "enemyBullet",
			Height = 40,
			Width = 15,
			Fill = Brushes.Yellow,
			Stroke = Brushes.Black,
			StrokeThickness = 5
		};
		// now we place the bullets top location to the Y variable
		Canvas.SetTop(newEnemyBullet, y);
		// set the left location to the X location
		Canvas.SetLeft(newEnemyBullet, x);
		// add the bullet to the screen
		myCanvas.Children.Add(newEnemyBullet);
	}

	private static ImageSource GetImageSource(byte[] array)
	{
		var biImg = new BitmapImage();
		var ms = new MemoryStream(array);
		biImg.BeginInit();
		biImg.StreamSource = ms;
		biImg.EndInit();
		return biImg;
	}

	private void MakeEnemies(int limit)
	{ // make a local integer called left and set to 0
		var left = 0;
		// save the enemy limit as the total enemy
		_totalEnemeis = limit;
		// this is the for loop that will make all of the enemies for this game
		// if the limit is set to 10 this loop will run 10 times if set 20 to then 20 times and so on
		for (var i = 0; i < limit; i++)
		{
			// with each loop 
			// will create a new enemy skin image brush to be used with the enemy rectangle
			var enemySkin = new ImageBrush();
			// make a new rectangle called new enemy
			// inside this rectangle we set the properties to tag called enemy 45 height and width and link the enemy skin as the fill
			var newEnemy = new Rectangle
			{
				Tag = "enemy",
				Height = 45,
				Width = 45,
				Fill = enemySkin,
			};
			// set the starting location of the space inavder
			Canvas.SetTop(newEnemy, 10); // this is the top location
			Canvas.SetLeft(newEnemy, left); // this is the left location
			// add one to the scene 
			myCanvas.Children.Add(newEnemy);
			// change the to -60
			left -= 60;
			// add 1 to the enemy images integer
			_enemyImages++;
			// if enemy images integer goes above 8 
			// then we set the integer back to 1
			if (_enemyImages > 8)
			{
				_enemyImages = 1;
			}
			// the switch statement below is checking the enemy images integer
			// with each number it will assign a new skin to the enemy
			// this switch statement will run throughout the loop and it will help us make use of those space invader images we imported earlier
			// it will look for what number is in the enemy images integer and then assign that image to the enemy skin class and then break the loop. 
			switch (_enemyImages)
			{
				case 1:
					enemySkin.ImageSource = _invadersList[0];
					// if the enemy images number comes up as 1 we can change the image source to the invader 1 GIF file
					break;
				case 2:
					enemySkin.ImageSource = _invadersList[1];
					// if the enemy images number comes up as 1 we can change the image source to the invader 2 GIF file
					break;
				case 3:
					enemySkin.ImageSource = _invadersList[2];
					// if the enemy images number comes up as 1 we can change the image source to the invader 3 GIF file
					break;
				case 4:
					enemySkin.ImageSource = _invadersList[3];
					// if the enemy images number comes up as 1 we can change the image source to the invader 4 GIF file
					break;
				case 5:
					enemySkin.ImageSource = _invadersList[4];
					// if the enemy images number comes up as 1 we can change the image source to the invader 5 GIF file
					break;
				case 6:
					enemySkin.ImageSource = _invadersList[5];
					// if the enemy images number comes up as 1 we can change the image source to the invader 6 GIF file
					break;
				case 7:
					enemySkin.ImageSource = _invadersList[6];
					// if the enemy images number comes up as 1 we can change the image source to the invader 7 GIF file
					break;
				case 8:
					enemySkin.ImageSource = _invadersList[7];
					// if the enemy images number comes up as 1 we can change the image source to the invader 8 GIF file
					break;
			}
		}
	}
	private void GameEngine(object? sender, EventArgs e)
	{  // this is the game engine event, this event will trigger every 20 milliseconds with the timer tick
		// to begin we start with declaring a rect class linking it back to the player 1 rectangle we made in the canvas
		var player = new Rect(Canvas.GetLeft(player1), Canvas.GetTop(player1), player1.Width, player1.Height);
		// show the remaining space invader numbers on the screen with enemies left label
		enemiesLeft.Text = "Invaders Left: " + _totalEnemeis;
		// below is the player movement script
		// in the if statement below we are checking if the player is still inside the boundary from the left position
		// if so then we can move the player to towards left of the screen
		if (_goLeft && Canvas.GetLeft(player1) > 0)
		{
			Canvas.SetLeft(player1, Canvas.GetLeft(player1) - 10);
		}
		// in the if statement below we are checking if the players left position plus 65 pixels is still inside the main application window from the right
		// if so we can move the player towards the right of the screen
		else if (_goRight && Canvas.GetLeft(player1) + 80 < Application.Current.MainWindow.Width)
		{
			Canvas.SetLeft(player1, Canvas.GetLeft(player1) + 10);
		}
		//decrease 3 from the bullet timer interger every 20 milliseconds
		_bulletTimer -= 3;
		// when the bullet timer integer reaches below 0
		// run the enemy bullet maker function and tell it where to place the bullet on screen
		if (_bulletTimer < 0)
		{
			// we want the enemy bullet to be placed directly above the player character
			// this is why we are passing the player left position + 20 pixels
			// and the top position will be 10
			EnemyBulletMaker((Canvas.GetLeft(player1) + 20), 10);
			// reset the bullet timer back to bullet timer limit value
			_bulletTimer = BulletTimerLimit;
		}
		// if the total enemies number goes below 10
		// set the enemy speed to 20
		if (_totalEnemeis < 10)
		{
			_enemySpeed = 20;
		}
		// below is the code for collision detection between enemy, bullets, player and enemy bullets
		// run the foreach loop make a local variable x and scan through all of the rectangles available in my canvas
		foreach (var x in myCanvas.Children.OfType<Rectangle>())
		{
			// if any rectangle has the tag bullet in it
			if ((string)x.Tag == "bullet")
			{
				// move the bullet rectangle towards top of the screen
				Canvas.SetTop(x, Canvas.GetTop(x) - 20);
				// make a rect class with the bullet rectangles properties
				var bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
				// check if bullet has left top part of the screen
				if (Canvas.GetTop(x) < 10)
				{
					// if it has then add it to the item to remove list
					_itemstoremove.Add(x);
				}
				// run another for each loop inside of the main loop this one has a local variable called y
				foreach (var y in myCanvas.Children.OfType<Rectangle>())
				{
					// if y is a rectangle and it has a tag called enemy
					if ((string)y.Tag == "enemy")
					{
						// make a local rect called enemy and put the enemies properties into it
						var enemy = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
						// now check if bullet and enemy is colliding or not
						// if the bullet is colliding with the enemy rectangle
						if (bullet.IntersectsWith(enemy))
						{
							// remove the bullet, remove the enemy and deduct 1 from the total enemies integer
							_itemstoremove.Add(x);
							_itemstoremove.Add(y);
							_totalEnemeis -= 1;
						}
					}
				}
			}
			// we are back in the main loop again, this timer we need to animate the enemies
			// check again if the any rectangle has the tag enemy inside it
			if ((string)x.Tag == "enemy")
			{
				// move it towards right side of the screen with the enemy speed integer
				Canvas.SetLeft(x, Canvas.GetLeft(x) + _enemySpeed);
				// if the enemeies have left the screen from the right
				if (Canvas.GetLeft(x) > 820)
				{
					// position it back in the left
					Canvas.SetLeft(x, -80);
					// move it down the screen by 20 pixels
					Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
				}
				// make another local rect called enemy and put the new enemy properites into it
				var enemy = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
				// check if the player character and the enemy are colliding
				if (player.IntersectsWith(enemy))
				{
					// stop the timer and show a message that says you lose end game here
					_dispatcherTimer.Stop();
					MessageBox.Show("you lose");
				}
			}
			// back at the main game loop again we need to check for enemy bullets now
			// check if any rectangle has the enemyBullet tag inside of it
			if ((string)x.Tag == "enemyBullet")
			{
				// if we have found it then we will drop it towards bottom of the screen
				Canvas.SetTop(x, Canvas.GetTop(x) + 10);
				// if the bullet has gone passed the screen then we can add it to the remove list
				if (Canvas.GetTop(x) > 480)
				{
					_itemstoremove.Add(x);
				}
				// make a new local rect called enemy bullets and put the enemy bullets properites into it
				var enemyBullets = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
				// check if the enemy bullet or the player rectangle is colliding
				if (enemyBullets.IntersectsWith(player))
				{
					// if so stop the timer and show you lose message
					// game ends here
					_dispatcherTimer.Stop();
					MessageBox.Show("you lose");
				}
			}
		}
		// this is the garbage collection loop
		// check for every rectangle thats added to the itemstoremove list
		foreach (var y in _itemstoremove)
		{
			// remove them permanently from the canvas
			myCanvas.Children.Remove(y);
		}
		// if total enemies is 0
		if (_totalEnemeis < 1)
		{
			// stop the timer and show you win message
			_dispatcherTimer.Stop();
			MessageBox.Show("you win");
		}
	}
}