Shader "Custom/Cutout" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        /*Tags {"Queue" = "Geometry-1"}

        Pass
        {
            ZTest LEqual
            Zwrite On
            ColorMask 0
        }*/

        Tags {"Queue" = "Geometry-1"}

        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
    
            ZTest Always
            Zwrite Off
            ColorMask 0
        }
    }
}