//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace OpenGL_Game.Objects
//{
//    public class Bullet
//    {
//        static List<Entity> _bullet;
        
//        public bool Dead { get; set; }
//        public Vector Direction { get; set; }
//        public double Speed { get; set; }

//        public double X
//        {
//            get { return _bullet.GetPosition().X; }
//        }

//        public double Y
//        {
//            get { return _bullet.GetPosition().Y; }
//        }

//        public double Z
//        {
//            get { return _bullet.GetPosition().Z; }
//        }

//        public void SetPosition(Vector position)
//        {
//            _bullet.SetPosition(position);
//        }

//        public Bullet()
//        {            
//            Dead = false;
//            Direction = new Vector(1, 0, 0);
//            Speed = 512;
//        }

//        public void OnRender()
//        {
//            if (Dead)
//            {
//                return;
//            }
            
//        }

//        public void Update(double elapsedTime)
//        {
//            if (Dead)
//            {
//                return;
//            }
//            Vector position = _bullet.GetPosition();
//            position += Direction * Speed * elapsedTime;
//            _bullet.SetPosition(position);
//        }
//    }
//}
