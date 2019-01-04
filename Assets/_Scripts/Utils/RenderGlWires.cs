//

using UnityEngine;

namespace Utils{
    public class RenderGLWires
    {
        void OnPreRender()
        {
            GL.wireframe = true;
        }
        void OnPostRender()
        {
            GL.wireframe = false;
        }
    }   
}
