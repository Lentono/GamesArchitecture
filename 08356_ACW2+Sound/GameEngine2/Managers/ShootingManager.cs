//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;
//using GameEngine.Objects;
//using GameEngine.Scripts;
//using GameEngine.Components;

//namespace GameEngine.Managers
//{
    //Hadrien Orhand-Baron
//    public class ShootingManager
//    {
//        List<Bullet> _bullets = new List<Bullet>();

//        public void Shoot(Bullet bullet)
//        {
//            _bullets.Add(bullet);
//        }

//        public void Update(double elapsedTime)
//        {
//            UpdateBulletList(_bullets, elapsedTime);            
//        }

//        public void UpdateBulletList(List<Bullet> bulletList, double elapsedTime)
//        {
//            bulletList.ForEach(x => x.Update(elapsedTime));            
//            RemoveDeadBullets(bulletList);
//        }

//        private void RemoveDeadBullets(List<Bullet> bulletList)
//        {
//            for (int i = bulletList.Count - 1; i >= 0; i--)
//            {
//                if (bulletList[i].Dead)
//                {
//                    bulletList.RemoveAt(i);
//                }
//            }
//        }

//        void OnRender()
//        {
//            _bullets.ForEach(x => );
//        }

        
//    }
//}
