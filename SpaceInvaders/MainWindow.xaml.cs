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
	private bool _goLeft, _goRight;
	private int _bulletTimer;
	private const int BulletTimerLimit = 90;
	private const int EnemiesCount = 30;
	private int _enemySpeed = 6;

	private int _totalEnemies;
	private readonly DispatcherTimer _dispatcherTimer;
	
	private readonly DateTime _startTime;
	public MainWindow()
	{
		InitializeComponent();
		_dispatcherTimer = new DispatcherTimer();
		_dispatcherTimer.Tick += GameEngine;
		_dispatcherTimer.Interval = TimeSpan.FromMilliseconds(20);
		_dispatcherTimer.Start();
		Player1.Fill = new ImageBrush
		{
			ImageSource = GetImageSource(Properties.Resources.player)
		};
		MakeEnemies();
		_startTime = DateTime.Now;
	}

	private void Canvas_KeyIsDown(object sender, KeyEventArgs e)
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
			case Key.Left:
				_goLeft = false;
				break;
			case Key.Right:
				_goRight = false;
				break;
			case Key.Space:
			{
				var newBullet = new Rectangle
				{
					Tag = "bullet",
					Height = 20,
					Width = 5,
					Fill = Brushes.White,
					Stroke = Brushes.Red
				};

				Canvas.SetTop(newBullet, Canvas.GetTop(Player1) - newBullet.Height);
				Canvas.SetLeft(newBullet, Canvas.GetLeft(Player1) + Player1.Width / 2);

				MyCanvas.Children.Add(newBullet);
				break;
			}
		}
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

	private void MakeEnemies()
	{ 
		var left = 0;
		SetTotalEnemies(EnemiesCount);
		var invadersList = new List<ImageSource>
		{
			GetImageSource(Properties.Resources.invader1), GetImageSource(Properties.Resources.invader2),
			GetImageSource(Properties.Resources.invader3), GetImageSource(Properties.Resources.invader4),
			GetImageSource(Properties.Resources.invader5),
			GetImageSource(Properties.Resources.invader6), GetImageSource(Properties.Resources.invader7),
			GetImageSource(Properties.Resources.invader8)
		};

		for (var i = 0; i < EnemiesCount; i++)
		{
			var newEnemy = new Rectangle
			{
				Tag = "enemy",
				Height = 45,
				Width = 45,
				Fill = new ImageBrush
				{
					ImageSource = invadersList[i % 8]
				}
			};
			Canvas.SetTop(newEnemy, 30); 
			Canvas.SetLeft(newEnemy, left); 
			MyCanvas.Children.Add(newEnemy);
			left -= 60;
		}
	}


	private void GameEngine(object? sender, EventArgs e)
	{
		var time = DateTime.Now - _startTime;
		TimeCounter.Text = $"Time: {time.Seconds}.{time.Milliseconds}";
		if (_goLeft && Canvas.GetLeft(Player1) > 0)
			Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) - 10);
		else 
		if (_goRight && Canvas.GetLeft(Player1) + 65 < MyCanvas.ActualWidth)
			Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) + 10);

		_bulletTimer -= 3;
		if (_bulletTimer < 0)
		{
			var newEnemyBullet = new Rectangle
			{
				Tag = "enemyBullet",
				Height = 40,
				Width = 15,
				Fill = Brushes.Yellow,
				Stroke = Brushes.Black,
				StrokeThickness = 5
			};
			Canvas.SetTop(newEnemyBullet, 10);
			Canvas.SetLeft(newEnemyBullet, Canvas.GetLeft(Player1) + 20);
			MyCanvas.Children.Add(newEnemyBullet);
			_bulletTimer = BulletTimerLimit;
		}
		
		var elementsList = MyCanvas.Children.OfType<Rectangle>().ToList();
		foreach (var x in elementsList)
		{
			var tag = (string)x.Tag;
			switch (tag)
			{
				case "bullet":
				{
					Canvas.SetTop(x, Canvas.GetTop(x) - 20);
					var bullet = GetRect(x);
					foreach (var y in elementsList)
					{
						if ((string)y.Tag == "enemy" && bullet.IntersectsWith(GetRect(y)))
						{
							MyCanvas.Children.Remove(y);
							MyCanvas.Children.Remove(x);
							SetTotalEnemies(_totalEnemies - 1);
							break;
						}
					}
					if (Canvas.GetTop(x) < 10)
						MyCanvas.Children.Remove(x);
					break;
				}
				case "enemy":
				{
					Canvas.SetLeft(x, Canvas.GetLeft(x) + _enemySpeed);
					if (Canvas.GetLeft(x) > 820)
					{
						Canvas.SetLeft(x, -80);
						Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
					}
					IsGameLosing(x);
					break;
				}
				case "enemyBullet":
				{
					Canvas.SetTop(x, Canvas.GetTop(x) + 10);
					if (Canvas.GetTop(x) > 480) MyCanvas.Children.Remove(x);
					IsGameLosing(x);
					break;
				}
			}
		}
	}

	private void IsGameLosing(Rectangle x)
	{
		if (CheckPlayerIntersectsWith(GetRect(x)))
		{
			_dispatcherTimer.Stop();
			MessageBox.Show("You lose");
		}
	}

	private static Rect GetRect(Rectangle rectangle) => new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

	private bool CheckPlayerIntersectsWith(Rect rect1) => rect1.IntersectsWith(new Rect(Canvas.GetLeft(Player1), Canvas.GetTop(Player1), Player1.Width, Player1.Height));

	private void SetTotalEnemies(int value)
	{
		_totalEnemies = value;
		EnemiesLeft.Text = "Invaders Left: " + _totalEnemies;
		if (_totalEnemies < 10) _enemySpeed = 12;
		if (_totalEnemies < 1)
		{
			_dispatcherTimer.Stop();
			MessageBox.Show("You win!");
		}
	}
}