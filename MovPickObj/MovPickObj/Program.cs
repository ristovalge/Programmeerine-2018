using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;

namespace SFMLApp
{
    // Mängija liikumis suunad
    public enum Direction
    {
        None = 0,
        North = 1,
        NorthEast = 3,
        East = 2,
        SouthEast = 6,
        South = 4,
        SouthWest = 12,
        West = 8,
        NorthWest = 9
    }

    // Punk klassi definitsioon
    public class Point2f
    {
        public float x, y;
        public Point2f(float pX, float pY) { Set(pX, pY); }
        void Set(float pX, float pY) { x = pX; y = pY; }
    }

    // Mängu objekt, Kasuatatkse staatiliste objektide jaoks
    // koordinaadid arvestatakse vasakust ülemisest nurgast, laiesu ja kõrgusega
    // koosneb RectangleShape joonistamise infoga


    public class GameObject : Drawable
    {
        protected float x, y, w, h;
        public RectangleShape Rect { get; private set; }

        public float X { get => x; set { x = value; Rect.Position = new Vector2f(x, Rect.Position.Y); } }
        public float Y { get => y; set { y = value; Rect.Position = new Vector2f(Rect.Position.X, y); } }
        public float W { get => w; set { w = value; Rect.Size = new Vector2f(value, Rect.Size.Y); } }
        public float H { get => h; set { h = value; Rect.Size = new Vector2f(Rect.Size.X, value); } }

        public GameObject(float pX, float pY, float pW, float pH)
        {
            x = pX;
            y = pY;
            w = pW;
            h = pH;
            Rect = new RectangleShape(new Vector2f(w, h));
            Rect.Position = new Vector2f(pX, pY);
        }

        // Teise objektiga ristumise(collision) kontroll
        public bool Intersects(GameObject obj)
        {
            return (this.x <= (obj.x + obj.w) && this.x + w >= obj.x) && (this.x <= (obj.x + obj.w) && ((this.x + w) > obj.x));
        }

        // Teise objektiga ristumis kontroll kasutades etteantud koordinate
        public bool Intersects(float pX, float pY, float pW, float pH)
        {
            return (this.x <= (pX + pW) && this.x + w >= pX) && (this.y <= (pY + pH) && ((this.y + h) > pY));
        }
        // Ristumine(kollisiooni) suuna määramine

        public Direction CollisionDir(GameEntity obj)
        {
            Direction result = Direction.None;

            // Получаем спроецированную точку , куда необходимо переместить объект
            Point2f MoveTo = obj.MoveProject(obj.Speed);
            // Проверяем, есть ли коллизия между текущим и перемещаемым объектом
            bool collides = Intersects(MoveTo.x, MoveTo.y, obj.W, obj.H);
            // Если коллизии нет - возвращаем Направление.Нет
            if (!collides) return Direction.None;

            float minX = this.X;
            float maxX = this.X + this.W;
            float minY = this.Y;
            float maxY = this.Y + this.H;

            // Находим центральные точки двух объектов
            Point2f midpoint_this = new Point2f(this.X + (this.W / 2), this.Y + (this.H / 2));
            Point2f midpoint_other = new Point2f(obj.X + (obj.W / 2), obj.Y + (obj.H / 2));

            // Находим разницу между двумя объектами
            double dx = midpoint_other.x - midpoint_this.x;
            double dy = midpoint_other.y - midpoint_this.y;

            // Если модуляь dx больше модуля dy, значит направление коллизии на оси Запад-Восток (горизонталь)
            if (Math.Abs(dx) >= Math.Abs(dy))
            {
                // Если dx >= 0, считаем направление коллизии как "Запад"
                if (dx >= 0) result = Direction.West;
                // В ином случае "Восток"
                else result = Direction.East;
            }
            // Если модуль dy больше, тогда направление коллизии - вертикальное
            else
            {
                if (dy > 0) result = Direction.North;
                else result = Direction.South;
            }

            return result;
        }

        // Metod mis pärineb classist Drawable, sellecks et GameObject võiks olla joonistatud ekraanile kasutades meetodit window.Draw(obj);
        public void Draw(RenderTarget target, RenderStates states)
        {
            target.Draw(Rect, states);
        }
    }

    // Baas klass TehisIntellekti "AI loomiseks"
    public abstract class EntityAI
    {
        protected GameEntity entity;
        protected GameWorld world;

        public EntityAI(GameEntity p_entity, GameWorld p_world)
        {
            entity = p_entity; // TI võimalused
            world = p_world; // mänguruum
        }

        // TehisIntellekti sammahaaval strateegia üks käik
        public abstract void Tick();
    }

    // Класс "игровая сущность", обладает скоростью, направлением и AI
    public class GameEntity : GameObject
    {
        public const float Sqrt2 = 1.41421356f;

        public float Speed = 0.0f;
        public Direction Dir = Direction.None;

        public EntityAI AI = null;

        public GameEntity(float pX, float pY, float pW, float pH) : base(pX, pY, pW, pH) { }

        // Сгенерировать точку, в которую будем двигаться с направлением this.Dir и указанной скоростью
        public Point2f MoveProject(float pSpeed)
        {
            float moveToX = 0.0f;
            float moveToY = 0.0f;

            switch (Dir)
            {
                case Direction.None:
                    moveToX = this.x;
                    moveToY = this.Y;
                    break;
                case Direction.North:
                    // x не меняется - только y против оси x (в экранных координатах направлена вниз)
                    moveToX = this.x;
                    moveToY = this.y - pSpeed;
                    break;
                case Direction.NorthEast:
                    // меняется и x и y, угол 45 градусов, так что движение поровно, делённое на корень из двух
                    moveToX = this.x + pSpeed / Sqrt2;
                    moveToY = this.y - pSpeed / Sqrt2;
                    break;
                case Direction.East:
                    moveToX = this.x + pSpeed;
                    moveToY = this.y;
                    break;
                case Direction.SouthEast:
                    moveToX = this.x + pSpeed / Sqrt2;
                    moveToY = this.y + pSpeed / Sqrt2;
                    break;
                case Direction.South:
                    moveToX = this.x;
                    moveToY = this.y + pSpeed;
                    break;
                case Direction.SouthWest:
                    moveToX = this.x - pSpeed / Sqrt2;
                    moveToY = this.y + pSpeed / Sqrt2;
                    break;
                case Direction.West:
                    moveToX = this.x - pSpeed;
                    moveToY = this.y;
                    break;
                case Direction.NorthWest:
                    moveToX = this.x - pSpeed / Sqrt2;
                    moveToY = this.y - pSpeed / Sqrt2;
                    break;
            }

            return new Point2f(moveToX, moveToY);
        }

        // Враппер для текущей скорости сущности
        public bool MoveEntity(GameWorld w, bool checkonly = false)
        {
            return MoveEntity(w, Speed, out Direction d, checkonly);
        }

        // Попробовать движение в указанном мире с указанной скоростью
        // Если выставлен "checkonly" - только делаем проверку, не пытаемся передвигаться
        public bool MoveEntity(GameWorld w, float pSpeed, out Direction bumpDirection, bool checkonly = false)
        {
            bumpDirection = Direction.None;
            if (pSpeed <= 0.0f) return false;


            Point2f moveTo = MoveProject(pSpeed);

            // Проверяем коллизию и её направление со спроецированными координатами в указанном игровом мире
            bool canMove = w.CheckCollision(this, moveTo.x, moveTo.y, this.W, this.H, out bumpDirection);

            // Если коллизии нет - производим перемещение
            if (canMove)
            {
                this.X = moveTo.x;
                this.Y = moveTo.y;
                return true;
            }
            return false;
        }

        // Если AI объекта обозначен - вызываем сеанс его "мышления"
        public void AITick()
        {
            if (AI != null) AI.Tick();
        }
    }

    // Класс "игровой мир"
    // Содержит ссылку на приложение, мир, сущность игрока, а также полный список статических объектов и сущностей (включая самого игрока)
    // Мир - тоже игровой объект
    public class GameWorld
    {
        public GameApp app; // аппликация(тех дела, ввод/вывод, отрисовка)
        public GameObject world;  // Игровое пространство(мир)
        public GameEntity player; // Управляемый клавишами субъект - игрок
        public List<GameObject> statics = new List<GameObject>(); // Массив статических объектов
        public List<GameEntity> entities = new List<GameEntity>();  // Массив двигающихся субъектов

        public GameWorld(GameApp p_app, float WorldW, float WorldH, float PlayerW, float PlayerH)
        {
            app = p_app;
            world = new GameObject(0, 0, WorldW, WorldH); // Создаём пространство размером WorldW * WorldH
            // Помещаем игрока в центр мира
            //player = new GameEntity(WorldW / 2 - PlayerW / 2, WorldH / 2 - PlayerH / 2, PlayerW, PlayerH);
            
            // mängja algus start
            player = new GameEntity(10, 350, PlayerW, PlayerH);
            entities.Add(player); // Добавить в пространство игрока
        }

        // Проверяем коллизию указанной сущности(объекта/субъекта) с границами мира и всеми объектам в мире
        public bool CheckCollision(GameEntity entity, float x, float y, float pW, float pH, out Direction bumpDirection)
        {
            bumpDirection = Direction.None;
            // Проверка пересечения объектов/субъектов (Check world boundaries collision)
            bool worldResult = x > world.X && (x + pW) < (world.X + world.W) && y > world.Y && (y + pH) < (world.Y + world.H);

            if (!worldResult)
            {
                // Если есть коллизия с границами мира, определяем направление (где объект выходит за рамки)
                if (x < world.X) bumpDirection |= Direction.West;
                else if (x + pW > (world.X + world.W)) bumpDirection |= Direction.East;
                else if (y + pH > (world.Y + world.H)) bumpDirection |= Direction.South;
                else if (y < world.Y) bumpDirection |= Direction.North;
            }

            // Проверяем коллизию со всеми статическими объектам, останавливаемся на первом столкновении
            bool staticsResult = true;
            foreach (GameObject stat in statics)
            {
                // Сначала проверяем пересечение
                bool r = stat.Intersects(x, y, pW, pH);
                if (r == true)
                {
                    staticsResult = false;
                    // Если пересечение есть, тогда определяем его направление
                    bumpDirection = stat.CollisionDir(entity);
                    break;
                }
            }

            // То же самое со всеми сущностями
            bool entitiesResult = true;
            foreach (GameEntity ent in entities)
            {
                if (entity == ent) continue;
                bool r = ent.Intersects(x, y, pW, pH);
                if (r == true)
                {
                    entitiesResult = false;
                    bumpDirection = ent.CollisionDir(entity);
                    break;
                }
            }

            // Результат - логическое И всех трёх проверок
            return worldResult && staticsResult && entitiesResult;
        }
    }

    public class GameApp
    {
        public const int W = 1200, H = 800;
        public const int Framerate = 60;

        RenderWindow window;
        Vector2f center;

        GameWorld world;
        Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        const int pW = 40, pH = 60;

        RectangleShape cursor;

        // Sound snd = new Sound(new SoundBuffer("plonn.wav"));

        public Stopwatch clock { get; private set; } = Stopwatch.StartNew();

        public GameApp()
        {
            // Создаём окно
            window = new RenderWindow(new VideoMode(W, H, 32), "Game app",
                Styles.Close | Styles.Titlebar);

            // Регистрируем обработчики событий закрытия и нажатия на клавишы
            window.Closed += new EventHandler(WindowClosed);
            window.KeyPressed += new EventHandler<KeyEventArgs>(KeyPressed);

            center = new Vector2f(W / 2.0f, H / 2.0f);

            // Грузим, регистрируем текстуры в словаре
            textures.Add("player", new Texture("player.png"));
            textures.Add("npc", new Texture("man_clipped.png"));
            textures.Add("ball", new Texture("ball.png"));
            textures.Add("rock", new Texture("wall.png"));

            // Создаём игровой мир
            world = new GameWorld(this, W, H, pW, pH);
        }

        public void Run()
        {
           // world.player = new GameEntity(center.X - pW / 1, center.Y - pH / 2, pW, pH);
            // Инициализируем игрока
            world.player.Speed = 5.0f;
            world.player.Rect.Texture = textures["player"];

            // Скрываем курсор в окне
            window.SetMouseCursorVisible(false);

            // Добавляем статичные объекты - камни
            List<GameObject> rocks = new List<GameObject>();
            rocks.Add(new GameObject(83, 77, 920, 32));
            rocks.Add(new GameObject(1000, 77, 32, 500));
            rocks.Add(new GameObject(1, 420, 310, 32));
            rocks.Add(new GameObject(280, 175, 32, 277));
            rocks.Add(new GameObject(900, 175, 32, 500));
            rocks.Add(new GameObject(83, 100, 32, 320));
            rocks.Add(new GameObject(180, 100, 32, 250));
            rocks.Add(new GameObject(173, 77, 32, 32));
            rocks.Add(new GameObject(528, 423, 32, 320));
            rocks.Add(new GameObject(528, 175, 380, 32));
            rocks.Add(new GameObject(528, 483, 32, 32));
            rocks.Add(new GameObject(528, 503, 32, 32));
            rocks.Add(new GameObject(705, 243, 32, 320));
            rocks.Add(new GameObject(94, 658, 460, 32));

            foreach (GameObject rock in rocks)
            {
                rock.Rect.Texture = textures["rock"];
                world.statics.Add(rock);
            }

            List<GameObject> balls = new List<GameObject>();
            balls.Add(new GameObject(130, 120, 32, 32));

            foreach (GameObject ball in balls)
            {
                ball.Rect.Texture = textures["ball"];
                world.statics.Add(ball);
            }


            // Добавляем неигровых персонажей (non player characters)
            List<GameEntity> npcs = new List<GameEntity>();
            npcs.Add(new GameEntity(128, 456, 40, 60));
            npcs.Add(new GameEntity(504, 640, 40, 60));
            npcs.Add(new GameEntity(382, 374, 40, 60));

            cursor = new RectangleShape(new Vector2f(10, 10));

    

            // snd.Play();
            // Основной цикл программы:
            // 1. Обработка событий
            // 2. Игровая логика
            // 3. Отрисовка и прочий вывод
            while (window.IsOpen)
            {
                // Вызывает обработчики собыйтий
                window.DispatchEvents();

                // Осуществляем передвижение игрока
                world.player.Dir = CalculateDirection();
                world.player.MoveEntity(world);

                // Рисуем курсор (белый прямоугольник)
                Vector2i mpos = SFML.Window.Mouse.GetPosition(window);
                cursor.Position = new Vector2f(mpos.X, mpos.Y);

                // Заливаем экран в зелёный цвет
                window.Clear(new Color(0x30, 0x90, 0x30, 0xff));

                // Отрисовка + АИ
                foreach (GameObject stat in world.statics) window.Draw(stat);
                foreach (GameEntity entity in world.entities)
                {
                    entity.AITick();
                    window.Draw(entity);
                }
                window.Draw(world.player);
                window.Draw(cursor);

                window.Display();

                // Ждём, пока не наступит время отрисовывать следующий кадр
                Thread.Sleep(1000 / Framerate);
            }
        }

        // Määrame mängija figuuri liikumise suunda klahvide vajutuse järgi
        Direction CalculateDirection()
        {
            Direction dir = Direction.None;
            if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.W)) dir |= Direction.North;
            if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.S)) if (dir.HasFlag(Direction.North)) dir -= Direction.North; else dir |= Direction.South;
            if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.A)) dir |= Direction.West;
            if (SFML.Window.Keyboard.IsKeyPressed(Keyboard.Key.D)) if (dir.HasFlag(Direction.West)) dir -= Direction.West; else dir |= Direction.East;

            return dir;
        }

        void WindowClosed(object sender, EventArgs e)
        {
            if (window != null) window.Close();
        }

        void KeyPressed(object sender, KeyEventArgs e)
        {
            float changeTo = 0;

            switch (e.Code)
            {
                case Keyboard.Key.Escape:
                    window.Close();
                    break;
                case Keyboard.Key.BackSpace:
                    world.player.X = world.world.W / 2 - world.player.W / 2;
                    world.player.Y = world.world.H / 2 - world.player.H / 2;
                    break;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameApp application = new GameApp();
            application.Run();
        }
    }
}
