﻿#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nuclex.UserInterface.Visuals.Flat {

  /// <summary>Provides drawing methods for GUI controls</summary>
  /// <remarks>
  ///   Analogous to System.Drawing.Graphics, but provides specialized methods for
  ///   drawing a GUI with a dynamic, switchable theme.
  /// </remarks>
  public interface IFlatGuiGraphics {

    /// <summary>Sets the clipping region for any future drawing commands</summary>
    /// <param name="clipRegion">Clipping region that will be set</param>
    /// <returns>
    ///   An object that will unset the clipping region upon its destruction.
    /// </returns>
    /// <remarks>
    ///   Clipping regions can be stacked, though this is not very typical for
    ///   a game GUI and also not recommended practice due to performance constraints.
    ///   Unless clipping is implemented in software, setting up a clip region
    ///   on current hardware requires the drawing queue to be flushed, negatively
    ///   impacting rendering performance (in technical terms, a clipping region
    ///   change likely causes 2 more DrawPrimitive() calls from the painter).
    /// </remarks>
    IDisposable SetClipRegion(RectangleF clipRegion);

    /// <summary>Draws a GUI element onto the drawing buffer</summary>
    /// <param name="frameName">Class of the element to draw</param>
    /// <param name="bounds">Region that will be covered by the drawn element</param>
    /// <remarks>
    ///   <para>
    ///     GUI elements are the basic building blocks of a GUI: 
    ///   </para>
    /// </remarks>
    void DrawElement(string frameName, RectangleF bounds);

    void DrawElement(string frameName, RectangleF bounds, Texture2D myTexture, Rectangle? clip);  // this line added by CSP

    void DrawElement(string frameName, RectangleF bounds, Texture2D myTexture, Rectangle? clip, Color color);

    /// <summary>Draws text into the drawing buffer for the specified element</summary>
    /// <param name="frameName">Class of the element for which to draw text</param>
    /// <param name="bounds">Region that will be covered by the drawn element</param>
    /// <param name="text">Text that will be drawn</param>
    void DrawString(string frameName, RectangleF bounds, string text);

    void DrawString(string frameName, RectangleF bounds, string text, Color color, SpriteFont font = null);

    /// <summary>Draws a caret for text input at the specified index</summary>
    /// <param name="frameName">Class of the element for which to draw a caret</param>
    /// <param name="bounds">Region that will be covered by the drawn element</param>
    /// <param name="text">Text for which a caret will be drawn</param>
    /// <param name="index">Index the caret will be drawn at</param>
    void DrawCaret(string frameName, RectangleF bounds, string text, int index);

    /// <summary>Measures the extents of a string in the frame's area</summary>
    /// <param name="frameName">Class of the element whose text will be measured</param>
    /// <param name="bounds">Region that will be covered by the drawn element</param>
    /// <param name="text">Text that will be measured</param>
    /// <returns>
    ///   The size and extents of the specified string within the frame
    /// </returns>
    RectangleF MeasureString(string frameName, RectangleF bounds, string text);

    /// <summary>
    ///   Locates the closest gap between two letters to the provided position
    /// </summary>
    /// <param name="frameName">Class of the element in which to find the gap</param>
    /// <param name="bounds">Region that will be covered by the drawn element</param>
    /// <param name="text">Text in which the closest gap will be found</param>
    /// <param name="position">Position of which to determien the closest gap</param>
    /// <returns>The index of the gap the position is closest to</returns>
    int GetClosestOpening(
      string frameName, RectangleF bounds, string text, Vector2 position
    );

  }

} // namespace Nuclex.UserInterface.Visuals.Flat
