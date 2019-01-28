namespace FSharpGephiStreamer

///Functions and types for color coding, decoding and conversions of hex colors
module Hex =
    
    open System

    ///Converts integer to hex based character (e.g. 1 -> '1', 11 -> 'B')
    [<CompiledName("ToHexDigit")>]
    let toHexDigit n =
        if n < 10 then char (n + 0x30) else char (n + 0x37)
    
    ///Converts a hex based character to an integer (e.g. '1' -> 1, 'B' -> 11)
    [<CompiledName("FromHexDigit")>]
    let fromHexDigit c =
        if c >= '0' && c <= '9' then int c - int '0'
        elif c >= 'A' && c <= 'F' then (int c - int 'A') + 10
        elif c >= 'a' && c <= 'f' then (int c - int 'a') + 10
        else raise <| new ArgumentException()
    
    ///Encodes a color byte array to a hex string with the given prefix 
    [<CompiledName("Encode")>]
    let encode (prefix:string) (color:byte array)  =
        let hex = Array.zeroCreate (color.Length * 2)
        let mutable n = 0
        for i = 0 to color.Length - 1 do
            hex.[n] <- toHexDigit ((int color.[i] &&& 0xF0) >>> 4)
            n <- n + 1
            hex.[n] <- toHexDigit (int color.[i] &&& 0xF)
            n <- n + 1
        String.Concat(prefix, new String(hex))
//        if prefix then String.Concat("0x", new String(hex)) 
//        else new String(hex)
            
    [<CompiledName("Decode")>]
    ///Decodes a color byte array from a hex string
    let decode (s:string) =
        match s with
        | null -> nullArg "s"
        | _ when s.Length = 0 -> Array.empty
        | _ ->
            let mutable len = s.Length
            let mutable i = 0
            if len >= 2 && s.[0] = '0' && (s.[1] = 'x' || s.[1] = 'X') then do
                len <- len - 2
                i <- i + 2
            if len % 2 <> 0 then invalidArg "s" "Invalid hex format"
            else
                let buf = Array.zeroCreate (len / 2)
                let mutable n = 0
                while i < s.Length do
                    buf.[n] <- byte (((fromHexDigit s.[i]) <<< 4) ||| (fromHexDigit s.[i + 1]))
                    i <- i + 2
                    n <- n + 1
                buf

//http://www.niwa.nu/2013/05/math-behind-colorspace-conversions-rgb-hsl/
///Module to create and manipulate ARGB colors
module Colors =
    
    /// Color component ARGB
    type ColorComponent =
        | A of byte
        | R of byte
        | G of byte
        | B of byte 
    
    /// returns the value hold by a color component
    let getValueFromCC cc =
        match cc with
        | A v -> v
        | R v -> v
        | G v -> v
        | B v -> v

    ///Represents an ARGB (alpha, red, green, blue) color
    type Color = {
        /// The alpha component value of this Color structure.
        A : byte
        /// The red component value of this Color structure.
        R : byte
        /// The green component value of this Color structure.
        G : byte
        /// The blue component value of this Color structure.
        B : byte
        }

    ///returns the maximum value of the R, G, and B components of a color
    let maxRGB c =
        let r,g,b = R c.R,G c.G,B c.B
        max r g |> max b

    ///returns the minimum value of the R, G, and B components of a color
    let minRGB c =
        let r,g,b = R c.R,G c.G,B c.B
        min r g |> min b
        


    /// Creates a Color structure from the four ARGB components (alpha, red, green, and blue) values.
    let fromArgb a r g b =
        let f v =
            if v < 0 || v > 255 then 
                failwithf "Value for component needs to be between 0 and 255."
            else
                byte v
        {A= f a; R = f r; G = f g; B = f b}

    /// Creates a Color structure from the specified color component values (red, green, and blue).
    /// The alpha value is implicitly 255 (fully opaque). 
    let fromRgb r g b =
        fromArgb 255 r g b

//    /// Gets the hue-saturation-brightness (HSB) brightness value for this Color structure.
//    let getBrightness = ()

    /// Gets the hue component value of the hue-saturation-brightness (HSB) format, in degrees, for this Color structure.
    let getHue c =
        let min = minRGB c |> getValueFromCC
        match maxRGB c with
        | R r -> float (c.G - c.B) / float (r - min)
        | G g -> 2.0 + float (c.B - c. R) / float (g - min)
        | B b -> 4.0 + float (c.R - c.G) / float (b - min)
        | _   -> failwithf "" // can't be


    /// Gets the saturation component value of the hue-saturation-brightness (HSB) format for this Color structure.
    let getSaturation col =
        let minimum = minRGB col
        let maximum = maxRGB col
        float (getValueFromCC minimum + getValueFromCC maximum) / 2.
        |> round
           
    /// Gets the 32-bit ARGB value of this Color structure.
    let toArgb c =
        (int c.A, int c.R, int c.G, int c.B)
    
    /// Gets the hex representataion (FFFFFF) of a color (with valid prefix "0xFFFFFF")
    let toHex prefix c =
        let prefix' = if prefix then "0x" else ""
        Hex.encode prefix' [|c.R;c.G;c.B|]                

    /// Gets color from hex representataion (FFFFFF) or (0xFFFFFF)
    let fromHex (s:string) =
        match (Hex.decode s) with
        | [|r;g;b|]  -> fromRgb (int r) (int g) (int b)
        | _          -> failwithf "Invalid hex color format"

    /// Gets the web color representataion (#FFFFFF)
    let toWebColor c =        
        Hex.encode "#" [|c.R;c.G;c.B|]                

    /// Gets color from web color (#FFFFFF)
    let fromWebColor (s:string) =
        let s' = s.TrimStart([|'#'|])
        match (Hex.decode s') with
        | [|r;g;b|]  -> fromRgb (int r) (int g) (int b)
        | _          -> failwithf "Invalid hex color format"


    /// Converts this Color structure to a human-readable string.
    let toString c =
        let a,r,g,b = toArgb c
        sprintf "{Alpha: %i Red: %i Green: %i Blue: %i}" a r g b

    
    
    // http://graphicdesign.stackexchange.com/questions/3682/where-can-i-find-a-large-palette-set-of-contrasting-colors-for-coloring-many-d
    ///Predefined colors
    module Table =    

        let black       = fromRgb   0   0   0                
        let blackLite   = fromRgb  89  89  89 // 35% lighter
        let white       = fromRgb 255 255 255

        /// Color palette from Microsoft office 2016
        module Office = 
        
            // blue
            let blue        = fromRgb  65 113 156        
            let lightBlue   = fromRgb 189 215 238
            let darkBlue    = fromRgb  68 114 196
                        
            // red           
            let red         = fromRgb 241  90  96  
            let lightRed    = fromRgb 252 212 214

            // orange           
            let orange      = fromRgb 237 125  49
            let lightOrange = fromRgb 248 203 173
                                                                  
            // yellow        
            let yellow      = fromRgb 255 217 102
            let lightYellow = fromRgb 255 230 153
            let darkYellow  = fromRgb 255 192   0
                         
            // green         
            let green       = fromRgb 122 195 106
            let lightGreen  = fromRgb 197 224 180
            let darkGreen   = fromRgb 112 173  71

            // grey         
            let grey        = fromRgb 165 165 165
            let lightGrey   = fromRgb 217 217 217

        // From publication: Escaping RGBland: Selecting Colors for Statistical Graphics
        // http://epub.wu.ac.at/1692/1/document.pdf
        ///Scientifically proven well distinguishable colors (http://epub.wu.ac.at/1692/1/document.pdf)
        module StatisticalGraphics24 =
        // 
            let Blue1       = fromRgb   2  63 165
            let Blue2       = fromRgb 125 135 185
            let Blue3       = fromRgb 190 193 212
            let Red1        = fromRgb 214 188 192
            let Red2        = fromRgb 187 119 132
            let Red3        = fromRgb 142   6  59
            let LightBlue1  = fromRgb  74 111 227
            let LightBlue2  = fromRgb 133 149 225
            let LightBlue3  = fromRgb 181 187 227
            let LightRed1   = fromRgb 230 175 185
            let LightRed2   = fromRgb 224 123 145
            let LightRed3   = fromRgb 211  63 106
            let Green1      = fromRgb  17 198  56
            let Green2      = fromRgb 141 213 147
            let Green3      = fromRgb 198 222 199
            let Orange1     = fromRgb 234 211 198
            let Orange2     = fromRgb 240 185 141
            let Orange3     = fromRgb 239 151   8
            let Cyan1       = fromRgb  15 207 192
            let Cyan2       = fromRgb 156 222 214
            let Cyan3       = fromRgb 213 234 231
            let Magenta1    = fromRgb 243 225 235
            let Magenta2    = fromRgb 246 196 225
            let Magenta3    = fromRgb 247 156 212

            let private rand = new System.Random()

            let private paletteArray =
                [|
                    Blue1     
                    Blue2     
                    Blue3     
                    Red1      
                    Red2      
                    Red3      
                    LightBlue1
                    LightBlue2
                    LightBlue3
                    LightRed1 
                    LightRed2 
                    LightRed3 
                    Green1    
                    Green2    
                    Green3    
                    Orange1   
                    Orange2   
                    Orange3   
                    Cyan1     
                    Cyan2     
                    Cyan3     
                    Magenta1  
                    Magenta2  
                    Magenta3  
                |]
            
            let getRandomColor() =
                let index = rand.Next(0,23)
                paletteArray.[index]
