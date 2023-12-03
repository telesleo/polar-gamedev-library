using Microsoft.Xna.Framework;

namespace Polar
{
    public class PolylineCollider : Collider {
        public Vector2[][] Polylines;

        public PolylineCollider(Vector2[][] polylines = default, bool @fixed = true) : base(@fixed) {
            Polylines = polylines;
        }

        public Vector2[][] GetWorldPolylines() {
            Vector2[][] worldPolylines = new Vector2[Polylines.Length][];
            if (Polylines == null) return worldPolylines;
            for (int i = 0; i < Polylines.Length; i++) {
                if (Polylines[i] == null) continue;
                worldPolylines[i] = new Vector2[Polylines[i].Length];
                for (int j = 0; j < Polylines[i].Length; j++)
                {
                    worldPolylines[i][j] = GameObject.Position + Polylines[i][j];
                }
            }
            return worldPolylines;
        }

        public override void DrawCollider() {
            //if (Polylines == null) return;
            //foreach (Vector2[] polyline in Polylines) {
            //    if (polyline == null) continue;
            //    for (int i = 0; i < polyline.Length - 1; i++) {
            //        drawer.DrawLine(GameObject.Position + polyline[i], GameObject.Position + polyline[i + 1], _colliderManager.DrawingColor);
            //    }
            //}
        }
    }
}
