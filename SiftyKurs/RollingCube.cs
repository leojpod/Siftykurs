using System;
using Sifteo;
using SiftyKurss;

namespace SiftyKurss
{
  public class RollingCube: GameCube
  {
    private int _x, _y;
    private int _height, _width;
    private readonly string _STONE;
    private float _speed_x;
    private float _speed_y;

    public RollingCube (Cube c): this(c,MIDDLE, MIDDLE,0,0) {}

    public RollingCube(Cube c, int x, int y): this(c, x, y, 0,0){}

    public RollingCube(Cube c, int x, int y, int initialSpeed_x, int initialSpeed_y) : base(c)
    {
      _c.userData = this;
      _x = x;
      _y = y;
      _speed_x = initialSpeed_x;
      _speed_y = initialSpeed_y;
      _STONE = "BallStone";
      _height = Cube.SCREEN_HEIGHT / 4;
      _width = Cube.SCREEN_WIDTH / 4;
    }

    override
    internal void SetupCube ()
    {
      base.SetupCube();
      DrawCube ();
     #region adding the events handling
      _c.TiltEvent += ChangeSpeed;
      //_c.NeighborAddEvent += PrepareCubeRolling;
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

    public void Refresh(float deltaTime){
      Log.Debug(deltaTime+" frame(s) spent since last Refresh");

      //speed is given in pixel per frame
      // and delta is given in frame which is perfect:

      _x += _speed_x*deltaTime;
      _y += _speed_y*deltaTime;

      //we just need to make sure that now _x and _y are still inside the limits...
      CheckBallPosition();
      DrawCube();
    }

    private void ChangeSpeed (Cube c, int x, int y, int z)
    {
      int[] tilt = Helper.NormalizeTilt(x, y, z);
      int t_x = tilt[0]; int t_y = tilt[1]; int t_z = tilt[2];

      _speedChange_x = speedFactor*t_x*(2-t_z);
      _speedChange_y = speedFactor*t_y*(2-t_z);
    }

    public void UpdateSpeed()
    {
      //TODO we might use this method to have a finner tunning of the speed variation
    }

    override
    protected void DrawCube (bool repaint)
    {
      base.DrawCube(false);
      int pic_x = _x - _width / 2;
      int pic_y = _y - _height / 2;
      Log.Debug ("placement values: x->" + _x + ", y->" + _y + ", pic_x->" + pic_x + ", pic_y->" + pic_y);
      _c.Image (_STONE, pic_x, pic_y, 0, 0, _width, _height, 1, 0);
      if (repaint) {
        _c.Paint ();
      }
    }

    private void CheckBallPosition(){
      //let's check X first:
      if(_x < 0){
        int min_x = base.MinX + _width/2;//the ball need to be inside the cube hence the _widht/2
        if( _x < min_x){
          _x = min_x;
        }//else it's fine!
      }else{ //i.e. _x >= 0
        int max_x = base.MaxX - _width/2;
        if(_x > max_x){
          _x = max_x;
        }//else it's fine!
      }

      //let's check Y now:
      if(_y < 0){
        int min_y = base.MinY + _height/2;
        if( _y < min_y){
          _y = min_y;
        }//else it's fine
      }else{
        int max_y = base.MaxY - _height/2;
        if( _y > max_y){
          _y = max_y;
        }
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

