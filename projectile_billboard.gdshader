shader_type spatial;

uniform sampler2D side_texture;
uniform sampler2D cross_texture;

bool should_show_cross(vec3 cameraPos, vec3 nodePos)
{
    vec3 direction_to_camera = normalize(cameraPos - nodePos);
    float angle_to_camera_y = atan(direction_to_camera.x,direction_to_camera.z);
    return (angle_to_camera_y > 1.2f);
}

void fragment() {
    if (should_show_cross(CAMERA_POSITION_WORLD, NODE_POSITION_WORLD))
    {
        vec4 albedo_tex = texture(cross_texture,UV);
        ALPHA = albedo_tex.a;
        ALBEDO = albedo_tex.rgb;
    }
    else
    {
        vec4 albedo_tex = texture(side_texture,UV);
        ALPHA = albedo_tex.a;
        ALBEDO = albedo_tex.rgb;
    }
}

void vertex() {
    if (should_show_cross(CAMERA_POSITION_WORLD, NODE_POSITION_WORLD))
    {
        MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
            INV_VIEW_MATRIX[0], INV_VIEW_MATRIX[1], INV_VIEW_MATRIX[2], MODEL_MATRIX[3]);
    }
    else
    {
        MODELVIEW_MATRIX = VIEW_MATRIX * mat4(
            MODEL_MATRIX[0], INV_VIEW_MATRIX[1], INV_VIEW_MATRIX[2], MODEL_MATRIX[3]);
    }
}