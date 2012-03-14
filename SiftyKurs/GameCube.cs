using System;
using Sifteo;

namespace SiftyKurss
{
  public class GameCube
  {
    protected Cube _c {
      get; private set;
    }
    private string _background;
    private string _border;
    protected int _borderSize{
      get; private set;
    }
    protected int _borderLength{
      get; private set;
    }
    protected bool[] _visibleBorders{
      get; private set;
    }
    public static readonly int MIDDLE = Cube.SCREEN_WIDTH / 2;

    public enum Side{TOP,LEFT,BOTTOM,RIGHT};

    public Cube C {
      get { return _c;}
    }
    public readonly float speedFactor;


    public GameCube (Cube c)
    {
      _c = c;
      _c.userData = this;
      _border = "borders";
      _background = "pavement";
      _borderSize = 8;
      _borderLength = Cube.SCREEN_WIDTH;
      _visibleBorders = new bool[]{true,true,true,true};
      speedFactor = 1;
    }

    protected void SetBorderDisplay(Cube.Side s, bool visible){
      switch(s){
      case Cube.Side.TOP:
        _visibleBorders[(int)GameCube.Side.TOP] = visible;
        break;
      case Cube.Side.LEFT:
        _visibleBorders[(int)GameCube.Side.LEFT] = visible;
        break;
      case Cube.Side.BOTTOM:
        _visibleBorders[(int)GameCube.Side.BOTTOM] = visible;
        break;
      case Cube.Side.RIGHT:
        _visibleBorders[(int)GameCube.Side.RIGHT] = visible;
        break;
      default:
        break;
      }
    }

    internal virtual void SetupCube(){
      _c.ClearEvents ();
      DrawCube();

      _c.NeighborAddEvent += RemoveBorder;
      _c.NeighborRemoveEvent += PutBackBorder;
    }

    void PutBackBorder (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {
      Log.Debug("adding back the border");
      SetBorderDisplay(side,true);
      DrawCube();
    }

    void RemoveBorder (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {
      Log.Debug("removing the border");
      SetBorderDisplay(side, false);
      DrawCube();
    }

    protected void DrawCube(){
      DrawCube(true);
    }

    protected virtual void DrawCube(bool repaint){
      //adding the background
      _c.Image (_background, 0, 0, 0, 0, Cube.SCREEN_WIDTH, Cube.SCREEN_HEIGHT, 1, 0);
      //adding the borders:
      if(_visibleBorders[(int)GameCube.Side.TOP]){
        _c.Image(_border, 0, 0, 0, 0, _borderLength, _borderSize, 1, 0 );
      }
      if(_visibleBorders[(int)GameCube.Side.LEFT]){
        _c.Image(_border, 0, 0, 0, 0, _borderSize, _borderLength, 1, 0 );
      }
      if(_visibleBorders[(int)GameCube.Side.BOTTOM]){
        _c.Image(_border, 0, Cube.SCREEN_MAX_Y-_borderSize,
                 0, Cube.SCREEN_MAX_Y-_borderSize, _borderLength, _borderSize, 1, 0 );
      }
      if(_visibleBorders[(int)GameCube.Side.RIGHT]){
        _c.Image(_border, Cube.SCREEN_MAX_X-_borderSize, 0,
                 Cube.SCREEN_MAX_X-_borderSize, 0, _borderSize, _borderLength, 1, 0 );
      }
      if(repaint){
        _c.Paint();
      }
    }
  }
}

