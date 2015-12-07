using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace ViewPortOpenGL
{
    public class ViewPortWindow
    {
        public Vector3[] pointsArray;
        public bool[] stimulusActive;

        public Vector3[] cubesArray;
        public int[] cubeColor;

        public bool BoxMode = false;

        public  ViewPortWindow()
        {
            //initPoints();
        }

        public void Run()
        {
            //initPoints();
            Color c = Color.MidnightBlue;
            Matrix4 m = Matrix4.Mult(Matrix4.Identity, Matrix4.CreateRotationY(Convert.ToSingle(3*Math.PI / 4)));
            m = Matrix4.Mult(m, Matrix4.CreateRotationX(Convert.ToSingle(-Math.PI/4)));
            Matrix4 DefaultMatrix = m;
            float pos = 0;
            float rot = 0;

            using (var game = new GameWindow())
            {
                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                    game.Title = "Space State 3D";
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.UpdateFrame += (sender, e) =>
                {
                    // add game logic, input handling
                    if (game.Mouse[MouseButton.Left])
                    {
                        m = DefaultMatrix;
                    }
                    if (game.Keyboard[Key.Escape])
                    {
                        game.Exit();
                    }
                    if (game.Keyboard[Key.W])
                    {
                        m=Matrix4.Mult(m,Matrix4.CreateTranslation(0, 0.1f, 0));
                        //pointsArray[0].Z = pointsArray[0].Z + 0.1f;
                    }
                    if (game.Keyboard[Key.S])
                    {
                        m=Matrix4.Mult(m,Matrix4.CreateTranslation(0, -0.1f, 0));
                        //pointsArray[0].Z = pointsArray[0].Z - 0.1f;
                    }
                    if (game.Keyboard[Key.A])
                    {
                        m = Matrix4.Mult(m, Matrix4.CreateTranslation(-0.1f, 0, 0));
                        //pointsArray[0].X = pointsArray[0].X - 0.1f;
                    }
                    if (game.Keyboard[Key.D])
                    {
                        m = Matrix4.Mult(m, Matrix4.CreateTranslation(+0.1f, 0, 0));
                        //pointsArray[0].X = pointsArray[0].X + 0.1f;
                    }


                    if (game.Keyboard[Key.Up])
                    {
                        //rot += .1f;
                        //m= Matrix4.CreateTranslation(pos, 0, 0);
                        m = Matrix4.Mult(m, Matrix4.CreateRotationX(.1f));
                    }
                    if (game.Keyboard[Key.Right])
                    {
                        m = Matrix4.Mult(m, Matrix4.CreateRotationY(.1f));
                    }
                    if (game.Keyboard[Key.Left])
                    {
                        m = Matrix4.Mult(m, Matrix4.CreateRotationY(-0.1f));
                    }
                    if (game.Keyboard[Key.Down])
                    {

                        m = Matrix4.Mult(m, Matrix4.CreateRotationX(-0.1f));
                    }
                };

                game.RenderFrame += (sender, e) =>
                {
                    // render graphics
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadMatrix(ref m);
                    //GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

                    // Draw the Axes
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color4(new Color4(255, 255, 255, 64));
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Vertex3(1f, 0f, 0f);
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Vertex3(0f, 1f, 0f);
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Vertex3(0f, 0f, 1f);
                    GL.End();
              

                    // Draw the Planes
                    /*
                    GL.Begin(PrimitiveType.Quads);
                    GL.Color4(new Color4(255, 255, 255, 64));
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Vertex3(1f, 0f, 0f);
                    GL.Vertex3(1f, 1f, 0f);
                    GL.Vertex3(0f, 1f, 0f);

                    GL.Vertex3(0f, 0f, 0f);
                    GL.Vertex3(1f, 0f, 0f);
                    GL.Vertex3(1f, 0f, 1f);
                    GL.Vertex3(0f, 0f, 1f);

                    
                    GL.Vertex3(0f, 0f, 0f);
                    GL.Vertex3(0f, 1f, 0f);
                    GL.Vertex3(0f, 1f, 1f);
                    GL.Vertex3(0f, 0f, 1f);
                    GL.End();*/

                    // Draw the points
                    if ((pointsArray != null) && !BoxMode)
                    {
                        GL.Begin(PrimitiveType.Points);
                        GL.Color3(Color.Red);
                        for (int i = 0; i < pointsArray.Length; i++)
                        {
                            if (stimulusActive[i])
                                GL.Color3(Color.Red);
                            else
                                GL.Color3(Color.Cyan);

                            if (i == pointsArray.Length - 1) // Last point added will be White
                                GL.Color3(Color.White);

                            GL.Vertex3(pointsArray[i]);
                        } 
                        GL.End();
                   
                    }

                    // Draw the Box
                    if ((pointsArray != null) && (BoxMode))
                    {
                        for (int i = 0; i < pointsArray.Length; i++)
                        {
                            GL.Color3(Color.FromArgb(cubeColor[i],cubeColor[i],cubeColor[i]));
                            DrawBox(pointsArray[i].X,
                                    pointsArray[i].Y,
                                    pointsArray[i].Z,
                                    0.025f);
                        }
                                                  
                    }

                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(30.0);
            }

        }


        public  void addPoints(float x, float y, float z,bool stimulusActive)
        {
            if (pointsArray == null)
            {
                Array.Resize(ref pointsArray, 1);
                Array.Resize(ref this.stimulusActive, 1);
                pointsArray[0] = new Vector3(x, y, z);
                this.stimulusActive[0] = stimulusActive;
            }
            else
            {
                Array.Resize(ref pointsArray, pointsArray.Length + 1);
                Array.Resize(ref this.stimulusActive, this.stimulusActive.Length + 1);
                Array.Resize(ref cubeColor, pointsArray.Length);
                pointsArray[pointsArray.Length - 1] = new Vector3(x, y, z);
                this.stimulusActive[this.stimulusActive.Length - 1] = stimulusActive;
            }

        }

        

        private void DrawBox(float x, float y, float z, float size)
        {
            float[,] n = new float[,]{
                {-1.0f, 0.0f, 0.0f},
                {0.0f, 1.0f, 0.0f},
                {1.0f, 0.0f, 0.0f},
                {0.0f, -1.0f, 0.0f},
                {0.0f, 0.0f, 1.0f},
                {0.0f, 0.0f, -1.0f}
                };

            int[,] faces = new int[,]{
                {0, 1, 2, 3},
                {3, 2, 6, 7},
                {7, 6, 5, 4},
                {4, 5, 1, 0},
                {5, 6, 2, 1},
                {7, 4, 0, 3}
                };

            float[,] v = new float[8, 3];
            int i;

            v[0, 0] = v[1, 0] = v[2, 0] = v[3, 0] = x-(size / 2);
            v[4, 0] = v[5, 0] = v[6, 0] = v[7, 0] = x+(size / 2);
            v[0, 1] = v[1, 1] = v[4, 1] = v[5, 1] = y-(size / 2);
            v[2, 1] = v[3, 1] = v[6, 1] = v[7, 1] = y+(size / 2);
            v[0, 2] = v[3, 2] = v[4, 2] = v[7, 2] = z-(size / 2);
            v[1, 2] = v[2, 2] = v[5, 2] = v[6, 2] = z+(size / 2);

            GL.Begin(PrimitiveType.Quads);
            for (i = 5; i >= 0; i--)
            {
                GL.Normal3(ref n[i, 0]);
                GL.Vertex3(ref v[faces[i, 0], 0]);
                GL.Vertex3(ref v[faces[i, 1], 0]);
                GL.Vertex3(ref v[faces[i, 2], 0]);
                GL.Vertex3(ref v[faces[i, 3], 0]);
            }
            
            GL.End();
            
        }
        
    }
}
