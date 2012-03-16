using System;
using Sifteo;
using SiftyKurs;

namespace SiftyKurs
{
  public class RollingCube: GameCube
  {
    private int _x, _y;
    private readonly int _height, _width;
    private readonly string _STONE;
    private double _speed_x;
    private double _speed_y;
    private double _speedChange_x;
    private double _speedChange_y;
    private CubeSwitchHandler switcher;

    public double SpeedX{
      get { return _speed_x;}
    }
    public double SpeedY{
      get { return _speed_y;}
    }
    public int X{
      get {return _x;}
    }
    public int Y{
      get {return _y;}
    }

    public RollingCube (Cube c): this(c,MIDDLE, MIDDLE,0,0) {}

    public RollingCube(Cube c, int x, int y): this(c, x, y, 0,0){}

    public RollingCube(Cube c, int x, int y, double initialSpeed_x, double initialSpeed_y) : base(c)
    {
      _c.userData = this;
      _x = x;
      _y = y;
      _speed_x = initialSpeed_x;
      _speed_y = initialSpeed_y;
      _speedChange_x = 0;
      _speedChange_y = 0;
      _STONE = "BallStone";
      _height = Cube.SCREEN_HEIGHT / 4;
      _width = Cube.SCREEN_WIDTH / 4;
    }

    internal delegate void BallOnBorderHandler(Cube.Side s);
    internal event BallOnBorderHandler BallOnBorderEvent;

    override
    internal void SetupCube ()
    {
      BallOnBorderEvent = null;
      base.SetupCube();
      DrawCube ();

     #region adding the events handling
      _c.TiltEvent += ChangeSpeed;
      _c.NeighborAddEvent += PrepareCubeRolling;
     #endregion
      //checking if any other cube is already our neighbor
      if(!_c.Neighbors.IsEmpty){
        foreach(Cube c in _c.Neighbors){
          if(c != null){
            Log.Debug("this rolling cube is already connected with another one");
            PrepareCubeRolling(_c, _c.Neighbors.SideOf(c), c, c.Neighbors.SideOf(_c));
          }
        }
      }
      //that's it for now

      Log.Debug("a rolling cube has been set up: speed("+_speed_x+";"+_speed_y+") position("+_x+";"+_y+")");
    }

    private void PrepareCubeRolling (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {
      Log.Debug("PrepareCubeRolling! cube side : "+side+" and neighbor side: "+neighborSide);
      //add a special tilt event listener that will swap the ball from this cube to the other
      switcher = new CubeSwitchHandler(this, side, neighbor, neighborSide);
      //TODO switch must listen the tick to be able to switch the ball of cube...
      this.BallOnBorderEvent +=  switcher.HandleBorderEvent;
      _c.NeighborRemoveEvent += switcher.Remove;
    }

    public void Refresh(float deltaTime){
      try{
        //speed is given in pixel per frame
        // and delta is given in frame which is perfect:
        int temp_x, temp_y;
        temp_x = _x + (int) (_speed_x*deltaTime);
        temp_y = _y + (int) (_speed_y*deltaTime);
        if(temp_x != _x || temp_y != _y){
          //Log.Debug(deltaTime+" frame(s) spent since last Refresh");
          //Log.Debug("current speed is: ("+_speed_x+","+_speed_y+")");
          _x = temp_x;
          _y = temp_y;

          //we just need to make sure that now _x and _y are still inside the limits...
          //Log.Debug("new ball position before calling checkBall -> "+_x+"x"+_y);
          CheckBallPosition();
          //Log.Debug("new ball position after calling checkBall -> "+_x+"x"+_y);
          DrawCube();
        }
      }catch (Exception e){
        Log.Debug("if the following message says that the ball switched cubes then it's perfect... \n\t-->"+e.Message);
      }
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
      _speed_x += _speedChange_x;
      _speed_y += _speedChange_y;
      //friction is missing here.... there should be something...
    }

    override
    protected void DrawCube (bool repaint)
    {
      base.DrawCube(false);
      int pic_x = _x - _width / 2;
      int pic_y = _y - _height / 2;
      //Log.Debug ("placement values: x->" + _x + ", y->" + _y + ", pic_x->" + pic_x + ", pic_y->" + pic_y);
      _c.Image (_STONE, pic_x, pic_y, 0, 0, _width, _height, 1, 0);
      if (repaint) {
        _c.Paint ();
      }
    }

    private void CheckBallPosition(){
      //let's check X first:
      if(_x < MIDDLE){
        int min_x = base.MinX + _width/2;//the ball need to be inside the cube hence the _widht/2
        if( _x < min_x){
          if(GoingTowardTheSide(Cube.Side.LEFT)){
            if(BallOnBorderEvent != null){
              BallOnBorderEvent(Cube.Side.LEFT);
            }
            //_speed_x = 0;//TODO find a formula to make the ball "bounce"
            //The BallOnBorder event MUST be send BEFORE changing the speed
            //... otherwise the ball will move in the wrong direction on the other cube in case of switch
            //... and it will loop like crazy...
            //something like speedFactor*0.5*_speed_x*-1 should do it;
            _speed_x = - 0.5*_speed_x;
          }
          _x = min_x;
        }//else it's fine!
      }else{ //i.e. _x >= 0
        int max_x = base.MaxX - _width/2;
        if(_x > max_x){
          if(GoingTowardTheSide(Cube.Side.RIGHT)){
            if(BallOnBorderEvent != null){
              BallOnBorderEvent(Cube.Side.RIGHT);
              Log.Debug("after a ball on the right border ");
            }
            _speed_x = - 0.5*_speed_x;
          }
          _x = max_x;
        }//else it's fine!
      }

      //let's check Y now:
      if(_y < MIDDLE){
        int min_y = base.MinY + _height/2;
        if( _y < min_y){
          if(GoingTowardTheSide(Cube.Side.TOP)){
            if(BallOnBorderEvent != null){
              BallOnBorderEvent(Cube.Side.TOP);
            }
            _speed_y = - 0.5*_speed_y;
          }
          _y = min_y;
        }//else it's fine
      }else{
        int max_y = base.MaxY - _height/2;
        if( _y > max_y){
          if(GoingTowardTheSide(Cube.Side.BOTTOM)){
            if(BallOnBorderEvent != null){
              BallOnBorderEvent(Cube.Side.BOTTOM);
            }
            _speed_y = - 0.5*_speed_y;
          }
          _y = max_y;
        }
      }
    }

    internal void MoveToNewCube(Cube futurRollingCube, int futur_x, int futur_y, double futur_speed_x, double futur_speed_y){
      _c.NeighborAddEvent -= PrepareCubeRolling;
      switcher.Remove();
      Log.Debug("Before swithing cube the old values are: speed("+_speed_x+";"+_speed_y+") position("+_x+";"+_y+")");

      this._c = futurRollingCube;
      _x = futur_x;
      _y = futur_y;
      _speed_x = futur_speed_x;
      _speed_y = futur_speed_y;
      _speedChange_x = 0;
      _speedChange_y = 0;
      this.SetupCube();
    }

    internal bool GoingTowardTheSide(Cube.Side s){
      bool goingTowardTheBorder = false;
      switch(s){
      case Cube.Side.BOTTOM:
        goingTowardTheBorder = (SpeedY > 0);
        break;
      case Cube.Side.TOP:
        goingTowardTheBorder = (SpeedY < 0);
        break;
      case Cube.Side.LEFT:
        goingTowardTheBorder = (SpeedX < 0);
        break;
      case Cube.Side.RIGHT:
        goingTowardTheBorder = (SpeedX > 0);
        break;
      default:
        break;
      }
      return goingTowardTheBorder;
    }
  }


}

