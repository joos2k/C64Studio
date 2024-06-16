﻿using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;



namespace DecentForms
{
  public class HScrollBar : ControlBase
  {
    public enum SBDisplayType
    {
      RAISED,
      FLAT
    }

    private Button    _TopButton = new Button();
    private Button    _BottomButton = new Button();

    private int       _ButtonWidth = 16;
    private int       _SliderWidth = 40;

    private bool      _MouseOverSlider = false;
    private bool      _SliderPushed = false;
    private int       _SliderDragOffset = 0;
    private int       _SliderDragPos = 0;
    private int       _MouseWheelFactor = 3;

    private int       _Value = 0;
    private int       _Minimum = 0;
    private int       _Maximum = 100;



    public int Minimum
    {
      get
      {
        return _Minimum;
      }
      set
      {
        if ( value < Maximum )
        {
          _Minimum = value;
          Value = _Value;
          Invalidate();
        }
      }
    }



    public int Maximum 
    {
      get
      {
        return _Maximum;
      }
      set
      {
        if ( value >= Minimum )
        {
          _Maximum = value;
          Value = _Value;
          Invalidate();
        }
      }
    }



    public int Value 
    {
      get
      {
        return _Value;
      }
      set
      {
        int     newValue = value;
        if ( newValue < Minimum )
        {
          newValue = Minimum;
        }
        if ( newValue > Maximum )
        {
          newValue = Maximum;
        }
        if ( newValue != _Value )
        {
          _Value = newValue;
          Invalidate();
        }
      }
    }



    public int SmallChange { get; set; } = 1;
    public int LargeChange { get; set; } = 10;



    public HScrollBar()
    {
      DisplayType = SBDisplayType.RAISED;
      BorderStyle = BorderStyle.NONE;
      Height = 17;

      _TopButton.CanBeFocused = false;
      _TopButton.Click += OnScrollUp;
      _TopButton.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
      _TopButton.CustomDraw += _TopButton_CustomDraw;

      _BottomButton.CanBeFocused = false;
      _BottomButton.Click += OnScrollDown;
      _BottomButton.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
      _BottomButton.CustomDraw += _BottomButton_CustomDraw;

      Controls.Add( _TopButton );
      Controls.Add( _BottomButton );
    }



    private void _TopButton_CustomDraw( ControlRenderer Renderer )
    {
      Renderer.RenderButton();

      var rect = _TopButton.Bounds;

      Renderer.DrawArrowUp( rect.X, rect.Y, rect.Width, rect.Height, Enabled );
    }



    private void _BottomButton_CustomDraw( ControlRenderer Renderer )
    {
      Renderer.RenderButton();

      int arrowWidth = 8;
      int arrowHeight = 4;
      int arrowX = ( _TopButton.Width - arrowWidth ) / 2;
      int arrowY = _TopButton.Height / 2 - 2;

      Renderer.DrawArrowDown( arrowX, arrowY, arrowWidth, arrowHeight, Enabled );
    }



    private void OnScrollUp( ControlBase Sender )
    {
      if ( Value > Minimum )
      {
        --Value;
        Invalidate();

        Scroll?.Invoke( this );
      }
    }



    private void OnScrollDown( ControlBase Sender )
    {
      if ( Value < Maximum )
      {
        ++Value;
        Invalidate();

        Scroll?.Invoke( this );
      }
    }



    public event EventHandler Scroll;



    public SBDisplayType DisplayType { get; set; }



    protected override void OnSizeChanged( System.EventArgs e )
    {
      base.OnSizeChanged( e );

      if ( _TopButton.Width == 0 )
      {
        _TopButton.Bounds = new System.Drawing.Rectangle( 0, 0, _ButtonWidth, ClientSize.Height );
        _BottomButton.Bounds = new System.Drawing.Rectangle( ClientSize.Width - _ButtonWidth, 0, _ButtonWidth, ClientSize.Height );
      }
      _ButtonWidth = _TopButton.Width;

      if ( ClientSize.Width > 2 * _ButtonWidth )
      {
        _SliderWidth = 40;
      }
      else
      {
        _SliderWidth = 20;
      }

      Invalidate();
    }



    protected override void OnControlEvent( ControlEvent Event )
    {
      switch ( Event.Type )
      {
        case ControlEvent.EventType.MOUSE_WHEEL:
          if ( Value - Event.MouseWheelDelta * _MouseWheelFactor < Minimum )
          {
            if ( Value > Minimum )
            {
              Value = Minimum;
              Invalidate(); 
              Scroll?.Invoke( this );
            }
          }
          else if ( Value - Event.MouseWheelDelta * _MouseWheelFactor > Maximum )
          {
            if ( Value < Maximum )
            {
              Value = Maximum;
              Invalidate();
              Scroll?.Invoke( this );
            }
          }
          else
          {
            Value -= Event.MouseWheelDelta * _MouseWheelFactor;
            Invalidate();
            Scroll?.Invoke( this );
          }
          UpdateMouseOverSlider( Event.MouseX, Event.MouseY );
          break;
        case ControlEvent.EventType.MOUSE_LEAVE:
          if ( _MouseOverSlider )
          {
            _MouseOverSlider = false;
            Invalidate();
          }
          break;
        case ControlEvent.EventType.MOUSE_UPDATE:
          _ButtonWidth = _TopButton.Width;
          if ( _SliderPushed )
          {
            // we're capturing
            int   newSliderPos = Event.MouseX - _ButtonWidth - _SliderDragOffset;
            if ( newSliderPos < 0 )
            {
              newSliderPos = 0;
            }
            if ( newSliderPos + _SliderWidth > ClientSize.Width - 2 * _ButtonWidth )
            {
              newSliderPos = ClientSize.Width - 2 * _ButtonWidth - _SliderWidth;
            }
            bool sliderPosChanged = _SliderDragPos != newSliderPos;
            _SliderDragPos = newSliderPos;
            int   newScrollPos = ( newSliderPos * ( Maximum - Minimum ) ) / ( ClientSize.Width - 2 * _ButtonWidth - _SliderWidth );
            if ( newScrollPos < Minimum )
            {
              newScrollPos = Minimum;
            }
            if ( newScrollPos > Maximum )
            {
              newScrollPos = Maximum;
            }

            if ( sliderPosChanged )
            {
              Invalidate();
              if ( Value != newScrollPos )
              {
                Value = newScrollPos;
                Scroll?.Invoke( this );
              }
            }
            break;
          }
          UpdateMouseOverSlider( Event.MouseX, Event.MouseY );
          break;
        case ControlEvent.EventType.MOUSE_DOWN:
          {
            _ButtonWidth = _TopButton.Width;

            int   sliderXPos = GetSliderRect().Left;

            Focus();

            if ( _MouseOverSlider )
            {
              if ( !_SliderPushed )
              {
                _SliderDragPos    = sliderXPos - _ButtonWidth;
                _SliderDragOffset = Event.MouseX - sliderXPos;
                _SliderPushed = true;
                Invalidate(); 
              }
              Capture = true;
            }
            else if ( Event.MouseX < sliderXPos )
            {
              // above slider
              ScrollBy( -LargeChange );
            }
            else
            {
              // below slider
              ScrollBy( LargeChange );
            }
          }
          break;
        case ControlEvent.EventType.MOUSE_UP:
          if ( _SliderPushed )
          {
            _ButtonWidth = _TopButton.Width;
            _SliderPushed = false;
            Capture = false;
            if ( !GetSliderRect().Contains( Event.MouseX, Event.MouseY ) )
            {
              _MouseOverSlider = false;
            }
            Invalidate();
          }
          break;
        case ControlEvent.EventType.KEY_DOWN:
          if ( Focused )
          {
            if ( Event.Key == Keys.Down )
            {
              OnScrollDown( null );
            }
            else if ( Event.Key == Keys.PageDown )
            {
              ScrollBy( LargeChange );
            }
            else if ( Event.Key == Keys.Home )
            {
              ScrollTo( 0 );
            }
            else if ( Event.Key == Keys.Up )
            {
              OnScrollUp( null );
            }
            else if ( Event.Key == Keys.PageUp )
            {
              ScrollBy( -LargeChange );
            }
            else if ( Event.Key == Keys.End )
            {
              ScrollTo( Maximum );
            }
          }
          break;
          /*
          case ControlEvent.EventType.KEY_DOWN:
            if ( Focused )
            {
              if ( Event.Key == Keys.Space )
              {
                if ( !_PushedByKey )
                {
                  _PushedByKey = true;
                  Invalidate();
                  Click?.Invoke( this );
                }
              }
            }
            break;
          case ControlEvent.EventType.KEY_UP:
            if ( Focused )
            {
              if ( Event.Key == Keys.Space )
              {
                if ( _PushedByKey )
                {
                  _PushedByKey = false;
                  Invalidate();
                }
              }
            }
            break;*/
      }
      base.OnControlEvent( Event );
    }



    public void ScrollBy( int Delta )
    {
      if ( Value + Delta > Maximum )
      {
        if ( Value != Maximum )
        {
          Value = Maximum;
          Invalidate();
          Scroll?.Invoke( this );
        }
      }
      else
      {
        Value += Delta;
        Invalidate();
        Scroll?.Invoke( this );
      }
    }



    public void ScrollTo( int TargetValue )
    {
      if ( TargetValue < 0 )
      {
        TargetValue = 0;
      }
      if ( TargetValue > Maximum )
      {
        TargetValue = Maximum;
      }
      if ( TargetValue != Value )
      {
        Value = TargetValue;
        Invalidate();
        Scroll?.Invoke( this );
      }
    }



    private void UpdateMouseOverSlider( int X, int Y )
    {
      if ( GetSliderRect().Contains( X, Y ) )
      {
        if ( !_MouseOverSlider )
        {
          _MouseOverSlider = true;
          Invalidate();
        }
      }
      else
      {
        if ( _MouseOverSlider )
        {
          _MouseOverSlider = false;
          Invalidate();
        }
      }
    }



    protected Rectangle GetSliderRect()
    {
      int     sliderXPos = 0;
      if ( _SliderPushed )
      {
        // during dragging the slider has a better resolution
        sliderXPos = _SliderDragPos;
      }
      else if ( Maximum - Minimum != 0 )
      {
        sliderXPos = ( ClientSize.Width - 2 * _ButtonWidth - _SliderWidth ) * Value / ( Maximum - Minimum );
      }

      return new Rectangle( _ButtonWidth + sliderXPos, 0, _SliderWidth, ClientSize.Height );
    }



    protected override void OnPaint( ControlRenderer Renderer )
    {
      if ( !Enabled )
      {
        return;
      }

      var sliderRect = GetSliderRect();
      Renderer.RenderSlider( sliderRect.Left, sliderRect.Top, sliderRect.Width, sliderRect.Height, _MouseOverSlider, _SliderPushed );
    }



    public void SetSliderSize( int SliderSize )
    {
      if ( SliderSize < 20 )
      {
        SliderSize = 20;
      }
      _SliderWidth = SliderSize;
      Invalidate();
    }



  }
}