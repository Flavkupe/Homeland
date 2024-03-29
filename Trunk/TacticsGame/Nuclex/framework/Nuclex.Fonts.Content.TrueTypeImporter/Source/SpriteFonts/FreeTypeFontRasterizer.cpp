#pragma region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2011 Nuclex Development Labs

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
#pragma endregion

#include "FreeTypeFontRasterizer.h"

using namespace std;

using namespace System;

using namespace Microsoft::Xna::Framework;
using namespace Microsoft::Xna::Framework::Graphics;
using namespace Microsoft::Xna::Framework::Content::Pipeline::Graphics;

namespace Nuclex { namespace Fonts { namespace Content {

  // ------------------------------------------------------------------------------------------- //

  FreeTypeFontRasterizer::FreeTypeFontRasterizer(
    Microsoft::Xna::Framework::Content::Pipeline::Graphics::FontDescription ^fontDescription,
    FontHinter hinter
  ) : FreeTypeFontProcessor(fontDescription, hinter) {}

  // ------------------------------------------------------------------------------------------- //

  Microsoft::Xna::Framework::Content::Pipeline::Graphics::PixelBitmapContent<
    Microsoft::Xna::Framework::Color
  > ^FreeTypeFontRasterizer::Rasterize(wchar_t character) {

    // Prepare the character in FreeType
    LoadCharacter(character);

    // Cache the glyph's bitmap to simplify the code below
    const FT_Bitmap &bitmap = this->freeTypeFace->glyph->bitmap;

    // Are there any pixels at all in the rendered glyph?
    if((bitmap.width > 0) && (bitmap.rows > 0)) {

      PixelBitmapContent<Color> ^rasterizedCharacter = gcnew PixelBitmapContent<Color>(
        bitmap.width, bitmap.rows
      );
      for(int y = 0; y < bitmap.rows; ++y) {
        unsigned char *lineStartAddress = bitmap.buffer + (bitmap.pitch * y);

        for(int x = 0; x < bitmap.width; ++x) {

          // Convert the color into premultiplied alpha
          unsigned char alpha = lineStartAddress[x];
          rasterizedCharacter->SetPixel(x, y, Color(alpha, alpha, alpha, alpha));

        }      
      }

      return rasterizedCharacter;

    } else { // The rendered glyph has zero size in at least one dimension

      return nullptr;

    }

  }

  // ------------------------------------------------------------------------------------------- //

  Microsoft::Xna::Framework::Point FreeTypeFontRasterizer::GetOffset(
    wchar_t character, FontOrigin origin
  ) {

    // Prepare the character in FreeType
    LoadCharacter(character);

    // Calculate the offset for the character on the Y axis based on whether
    // the user wants the font's upper end or the baseline at the pen position.
    int yOffset;
    if(origin == FontOrigin::UpperEnd) {
      yOffset = (this->freeTypeFace->size->metrics.ascender >> 6);
    } else {
      yOffset = 0;
    }

    // Return the bitmap offset informations stored in the glyph structure
    return Point(
      this->freeTypeFace->glyph->bitmap_left,
      yOffset - this->freeTypeFace->glyph->bitmap_top
    );

  }

  // ------------------------------------------------------------------------------------------- //

  Microsoft::Xna::Framework::Point FreeTypeFontRasterizer::GetAdvancement(wchar_t character) {

    // Prepare the character in FreeType
    LoadCharacter(character);

    // Return the bitmap offset informations stored in the glyph structure
    return Point(
      this->freeTypeFace->glyph->advance.x >> 6,
      this->freeTypeFace->glyph->advance.y >> 6
    );

  }

  // ------------------------------------------------------------------------------------------- //

}}} // namespace Nuclex::Fonts::Content
