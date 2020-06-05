using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Media;
namespace MarioGame
{
    class Creature : items

    {
        ////////////////////////////////////////////////////////// Creature Variables (NOT MARIO) //////////////////////////////////////////////
        public int Counter = 0;
        public bool isDead = false;
        public bool cdirection;
        public bool CisHit = false;
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Rectangle Xdesired;
        public Rectangle Ydesired;
        public int xjump;
        public bool Up, Left, Right;
        public bool IsJumping;
        public int FallSPeed;
        public float Force;
        public int JumpSpeed;
        public bool collisiontop;
        public bool collisionbot;
        public bool collisionright;
        public bool collisionleft;
        public int xspeed;
        bool isMoving;
        public int DeadCreatureCounter = 0;

        public Rectangle hitarea;
        public bool hitblock = false;
        public Creature(int x, int y, int w, int h, Image I) : base(x, y, w, h, I)
        {
            actual.X = x;
            actual.Y = y;
            actual.Width = w;
            actual.Height = h;
            IsJumping = false;
            FallSPeed = 0;
            Force = 0;

        }

        public void MoveMario(bool up, bool Left, bool Right, List<items> map, Form1 f, Image[] R, Image[] L)
        {
            if (!isDead)
            {
                if (Up)
                {

                    IsJumping = true;

                }

                if (Left || xspeed < 0)
                {
                    if (xspeed < -8)//sets your maximum running speed
                        xspeed = -8;
                    else if (Left)
                        xspeed -= 2;
                    CurrentImage = L[Counter];
                    cdirection = false;
                    Xdesired.X += xspeed;
                    UpdateActualX();
                }
                if (Right || xspeed > 0)//sets your maximum speed
                {
                    if (xspeed > 8)
                        xspeed = 8;
                    else if (Right)
                        xspeed += 2;
                    CurrentImage = R[Counter];
                    cdirection = true;
                    Xdesired.X += xspeed;
                    UpdateActualX();
                }
                if (xspeed == 0 && MarioCollideBot(map))
                    if (cdirection)
                        CurrentImage = R[2];
                    else
                        CurrentImage = L[2];
                if (JumpSpeed != 0 && !MarioCollideBot(map))
                    if (cdirection)
                        CurrentImage = R[3];
                    else
                        CurrentImage = L[3];

                if (actual.X + actual.Width - 20 < 0)
                    actual.X = f.ClientRectangle.Width - 20;
                if (actual.X + 20 > f.ClientRectangle.Width)
                    actual.X = 0 - actual.Width + 20;

            }
        }
        public void Inertia()
        {
            UpdateXDesired();

            if (xspeed > 0)
            {
                Xdesired.X += xspeed;

                xspeed -= 1;

            }
            else if (xspeed < 0)
            {
                Xdesired.X += xspeed;
                if (xspeed != 0)
                    xspeed += 1;

            }

            UpdateActualX();

        }
        public void UpdateYDesired()
        {
            Ydesired.X = actual.X;
            Ydesired.Y = actual.Y - 1;
            Ydesired.Width = actual.Width;
            Ydesired.Height = actual.Height + 2;
        }
        public void UpdateXDesired()
        {
            Xdesired.X = actual.X - 1;
            Xdesired.Y = actual.Y;
            Xdesired.Width = actual.Width + 2;
            Xdesired.Height = actual.Height;
        }
        public void UpdateActualY()
        {
            actual.X = Ydesired.X;
            actual.Y = Ydesired.Y + 1;
        }
        public void UpdateActualX()
        {
            actual.X = Xdesired.X + 1;
            actual.Y = Xdesired.Y;
        }

        public bool mariocollideright(List<items> map)
        {
            foreach (items x in map)
            {
                if (Xdesired.IntersectsWith(x.actual))
                    if (actual.X + (0.5 * actual.Width) < x.actual.X)
                    {
                        collisionright = true;
                        break;
                    }
                    else { }
                else
                    collisionright = false;

            }
            return collisionright;
        }
        public bool mariocollideleft(List<items> map)
        {
            foreach (items x in map)
            {
                if (Xdesired.IntersectsWith(x.actual))
                    if (actual.X + actual.Width > x.actual.X)
                    {
                        collisionleft = true;
                        break;
                    }
                    else { }
                else
                    collisionleft = false;

            }
            return collisionright;
        }
        public bool MarioCollideTop(List<items> map)
        {

            foreach (items x in map)
            {
                if (Ydesired.IntersectsWith(x.actual))
                    if (actual.Y >= x.actual.Y + x.actual.Height)
                    {
                        collisiontop = true;

                        break;
                    }
                    else { }
                else
                    collisiontop = false;
            }
            return collisiontop;
        }

        public bool MarioCollideBot(List<items> map)
        {

            foreach (items x in map)
            {
                if (Ydesired.IntersectsWith(x.actual))   // the desired's position intersecting with the map
                    if (actual.Y + actual.Height <= x.actual.Y) // mario's actual position is above the map
                    {
                        collisionbot = true;


                        break;
                    }
                    else { }
                else
                    collisionbot = false;
            }
            return collisionbot;

        }
      



        public void gravity(List<items> map)
        {

            UpdateYDesired();
            JumpSpeed = 30;//how fast you go up
            if (Force > 0)
            {

                //  int counter = 0;
                for (int i = 0; i < JumpSpeed; i++)
                {
                    Ydesired.Y--;
                    //if (counter < Math.Abs(xjump))
                    //if (xjump > 0)
                    //    desired.X++;
                    //else
                    //    desired.X--;
                    if (!MarioCollideTop(map))
                        UpdateActualY();
                    JumpSpeed--;
                    // counter+=1; // this is the resistance of air
                }
                Force -= 2; //how long you stay up
            }
            else
            {
                IsJumping = false;
                if (!MarioCollideBot(map))
                {
                    FallSPeed += 4; //gravity value here !
                    for (int i = 0; i < FallSPeed; i++)
                    {
                        Ydesired.Y++;
                        if (MarioCollideBot(map))
                        {
                            JumpSpeed = 0;
                            FallSPeed = 0;
                            break;
                        }

                    }
                    UpdateActualY();
                    UpdateYDesired();
                }
            }
        }

        public void MarioJump2(List<items> map, bool UpisPressed)
        {
            if (!isDead)
            {
                if (MarioCollideBot(map))
                    if (UpisPressed == true)
                    {
                        IsJumping = true;
                        Force = 35;
                        JumpSpeed = 0;
                        xjump = xspeed;


                    }
            }
        }
        public void HitMario(List<Creature> enemies, Image[] L)
        {
            foreach (Creature c in enemies)
                if (Ydesired.IntersectsWith(c.actual) && !c.CisHit)
                {

                    isDead = true;
                    CurrentImage = L[4];
                }

        }






        ///////////////////////////////////////////////////// CREATURES ///////////////////////////////////////////////////////////////////
        public void createCreatures(List<Creature> Enemies, int timertickcounter, Form1 f, Image I)
        {
            Point location;
            if (timertickcounter % 2 == 0)
            {
                location = new Point(120, 150);
                cdirection = true; // move right
            }
            else
            {
                location = new Point(f.ClientRectangle.Width - 165, 150);
                cdirection = false; // move left
            }
            Creature c = new Creature(location.X, location.Y, 45, 45, I);
            c.cdirection = cdirection;
            Enemies.Add(c);
        }

        public void moveCreature(List<Creature> Enemies, Form1 f, Image[] L, Image[] R)
        {
            foreach (Creature c in Enemies)
            {
                //this teleports turtles anywhere except on ground
                if (c.actual.X + c.actual.Width - 20 < 0)
                    c.actual.X = f.ClientRectangle.Width - 20;
                if (c.actual.X + 20 > f.ClientRectangle.Width)
                    c.actual.X = 0 - c.actual.Width + 20;

                //this to teleport tutles when they reach pipelines at ground
                if (c.actual.X <= 120 && (c.actual.Y <= f.ClientRectangle.Height - 60 && c.actual.Y >= 800))
                {
                    c.actual.Y = 150;
                    c.cdirection = !c.cdirection;
                }
                if (c.actual.X >= f.ClientRectangle.Width - 165 && (c.actual.Y <= f.ClientRectangle.Height - 60 && c.actual.Y >= 800))
                {
                    c.actual.Y = 150;
                    c.cdirection = !c.cdirection;
                }
                if (!c.CisHit)
                {
                    if (c.cdirection)
                    {
                        c.actual.X += 5;
                        c.CurrentImage = R[c.Counter];
                    }
                    else
                    {
                        c.actual.X -= 5;
                        c.CurrentImage = L[c.Counter];
                    }
                }
            }
        }
        public void HitCreature(List<Creature> Enemies, List<items> map, Image[] FlippedTurtle, SoundPlayer stomp)
        {

            for (int i = 0; i < Enemies.Count; i++)
            {
                if (MarioCollideTop(map))
                    if (Enemies[i].actual.X + Enemies[i].actual.Width / 2 > actual.X && Enemies[i].actual.X + Enemies[i].actual.Width / 2 < actual.X + actual.Width)
                        if (Enemies[i].actual.Y + Enemies[i].actual.Height + 35 > actual.Y && (actual.Bottom > Enemies[i].actual.Bottom))
                        {
                            Enemies[i].CisHit = true;
                            Enemies[i].Counter = 0;
                            Enemies[i].CurrentImage = FlippedTurtle[Enemies[i].Counter];

                        }
                if (actual.IntersectsWith(Enemies[i].actual) && Enemies[i].CisHit)
                {
                    Enemies.Remove(Enemies[i]);
                    stomp.Play();
                }

            }
        }

        public void HitBlock(List<items> map)
        {


            if (MarioCollideTop(map))
            {
                hitarea = new Rectangle(actual.X, actual.Y - 50, 60, 50);
                hitblock = true;
            }
            else
                hitblock = false;
        }

    }
}

