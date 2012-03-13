using System;
using Sifteo;
using SiftyKurss;

namespace SiftyKurss
{
  public class RollingCube: GameCube
  {
    private int _x, _y;
    private int _height, _width;
    private string _stone;

    public RollingCube (Cube c): base(c)
    {
      _c.userData = this;
      _x = Cube.SCREEN_WIDTH /2;
      _y = Cube.SCREEN_HEIGHT /2;
      _stone = "BallStone";
      _height = Cube.SCREEN_HEIGHT / 4;
      _width = Cube.SCREEN_WIDTH / 4;
    }


    override
    internal void SetupCube ()
    {
      base.SetupCube();
      DrawCube (true);
     #region adding the events handling
      _c.TiltEvent += ChangeCoordinate;
      _c.NeighborAddEvent += PrepareCubeRolling;
     #endregion
      //that's it for now
    }

    private void PrepareCubeRolling (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {
      Log.Debug("PrepareCubeRolling!");
      //add a special tilt event listener that will swap the ball from this cube to the other
      CubeSwitchHandler switcher = new CubeSwitchHandler(c, side, neighbor, neighborSide);
      _c.TiltEvent += switcher.Handle;
      _c.NeighborRemoveEvent += switcher.Remove;
    }


   
    private void ChangeCoordinate (Cube c, int x, int y, int z)
    {
      //tilt is just an extremly simple thing: each axis can only have 3 values
      // - 0 tilted on one side,
      // - 1 neutral,
      // - 2 tilted on the other side...
      // - except for z where 0 means that the cube is facing done, 2 facing up and 1 resting on one side
      //so it gave us a few options:
      // - we know where the stone is rolling by checking the value of x and y,
      // - z gaves us the "how much tilted information"
      // i.e. we wants something like :

      int[] tilt = Helper.NormalizeTilt (x, y, z);
      int t_x, t_y, t_z;
      t_x = tilt [0];
      t_y = tilt [1];
      t_z = tilt [2];
      int middle = Cube.SCREEN_WIDTH / 2;

      _x = (int)(middle * (1 + (t_x / (1.0 + t_z * t_z))) - t_x * (1 - t_z * t_z) * (_width / 2 + _borderSize) + (-1)*t_x*t_z*t_z*(_borderSize/2));
      _y = (int)(middle * (1 + (t_y / (1.0 + t_z * t_z))) - t_y * (1 - t_z * t_z) * (_width / 2 + _borderSize) + (-1)*t_y*t_z*t_z*(_borderSize/2));
      Log.Debug ("Normalized Tilt : x->" + t_x + ", y->" + t_y + ", z->" + t_z);
      DrawCube (true);
    }

    override
    protected void DrawCube (bool repaint)
    {
      base.DrawCube(false);
      int pic_x = _x - _width / 2;
      int pic_y = _y - _height / 2;
      Log.Debug ("placement values: x->" + _x + ", y->" + _y + ", pic_x->" + pic_x + ", pic_y->" + pic_y);
      _c.Image (_stone, pic_x, pic_y, 0, 0, _width, _height, 1, 0);
      if (repaint) {
        _c.Paint ();
      }
    }
  }

  internal class CubeSwitchHandler
  {
    private readonly Cube rollingCube;
    private readonly Cube.Side rollingCubeSide;
    private readonly Cube futurRollingCube;
    private readonly Cube.Side futurRollingCubeSide;


    internal CubeSwitchHandler(Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide){
      rollingCube = c;
      rollingCubeSide = side;
      futurRollingCube = neighbor;
      futurRollingCubeSide = neighborSide;
    }

    public void Handle(Cube c, int x, int y, int z){
      int[] tilt = Helper.NormalizeTilt(x, y, z);
      int t_x = tilt[0]; int t_y = tilt[1]; int t_z = tilt[2];

      if(t_z == 0){//i.e. if the cube is vertical
        //then the tilt must be on the right side to be interesting!
        switch(rollingCubeSide){
        case Cube.Side.BOTTOM:
          if(t_y == 1){
            SwitchCubes();
          }
          break;
        case Cube.Side.TOP:
          if(t_y == -1){
            SwitchCubes();
          }
          break;
        case Cube.Side.LEFT:
          if(t_x == -1){
            SwitchCubes();
          }
          break;
        case Cube.Side.RIGHT:
          if(t_x == 1){
            SwitchCubes();
          }
          break;
        default:
          break;
        }
      }//else nothing to do for now
    }

    public void Remove(Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide){
      //we don't care about the parameters...
      /* the point is:
       * - the cubes are no more neighbour
       * so we remove this object from the tilt listener list (and the neighbor remove one)
       * because we don't want the ball to switch cube anymore!
       */
      rollingCube.TiltEvent -= this.Handle;
      rollingCube.NeighborRemoveEvent -= this.Remove;
    }

    private void SwitchCubes(){
      //makes the rolling cube a simple GameCube
      GameCube gc = new GameCube(rollingCube);
      gc.SetupCube();
      //make the neighbor the new rolling cube
      RollingCube rc = new RollingCube(futurRollingCube);
      rc.SetupCube();
    }
  }
}

