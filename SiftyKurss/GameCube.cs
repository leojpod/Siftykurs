using System;
using Sifteo;

namespace SiftyKurss
{
  public class GameCube
  {
    private Cube _c {
      protected get;
    }
    private string _border;
    private int _borderSize;
    private int _borderLength;
    private bool[] _visibleBorders;
    public static readonly int MIDDLE = Cube.SCREEN_WIDTH / 2;

    enum Side{TOP,LEFT,BOTTON,RIGHT};

    public Cube C {
      get { return _c;}
    }

    public GameCube (Cube c)
    {
      _c = c;
      _c.userData = "JustACube";
      _border = "borders";
      _borderSize = 10;
      _borderLength = Cube.SCREEN_WIDTH;
      _visibleBorders = new bool[]{true,true,true,true};

    }

    protected void SetBorderDisplay(Cube.Side s, bool visible){
      switch(s){
      case Cube.Side.TOP:
        _visibleBorders[TOP] = visible;
        break;
      case Cube.Side.LEFT:
        _visibleBorders[LEFT] = visible;
        break;
      case Cube.Side.BOTTOM:
        _visibleBorders[BOTTOM] = visible;
        break;
      case Cube.Side.RIGHT:
        _visibleBorders[RIGHT] = visible;
        break;
      default:
        break;
      }
    }

    protected void SetupCube(){
      _c.ClearEvents ();
      DrawCube(false);

      _c.NeighborAddEvent += RemoveBorder;
      _c.NeighborRemoveEvent += PutBackBorder;
    }

    void PutBackBorder (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {
      SetBorderDisplay(side,true);
    }

    void RemoveBorder (Cube c, Cube.Side side, Cube neighbor, Cube.Side neighborSide)
    {
      SetBorderDisplay(side, false);
    }

    protected void DrawCube(bool repaint){
      //adding the background
      _c.Image ("pavement", 0, 0, 0, 0, Cube.SCREEN_WIDTH, Cube.SCREEN_HEIGHT, 1, 0);
      //adding the borders:
      if(_visibleBorders[TOP]){
        _c.Image(_border, 0, 0, 0, 0, _borderLength, _borderSize, 1, 0 );
      }
      if(_visibleBorders[LEFT]){
        _c.Image(_border, 0, 0, 0, 0, _borderSize, _borderLength, 1, 0 );
      }
      if(_visibleBorders[BOTTOM]){
        _c.Image(_border, 0, Cube.SCREEN_MAX_Y-_borderSize,
                 0, Cube.SCREEN_MAX_Y-_borderSize, _borderLength, _borderSize, 1, 0 );
      }
      if(_visibleBorders[RIGHT]){
        _c.Image(_border, Cube.SCREEN_MAX_X-_borderSize, 0,
                 Cube.SCREEN_MAX_X-_borderSize, 0, _borderSize, _borderLength, 1, 0 );
      }
      if(repaint){
        _c.Paint();
      }
    }
  }
}

