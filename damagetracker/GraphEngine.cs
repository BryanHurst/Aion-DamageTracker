using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace DamageTracker
{
    /// <summary>
    /// Draw live combat bar graphs in the main window
    /// </summary>
    public static class GraphEngine
    {
        //Paint the current dps meter
        public static void paint_meter(Graphics graphics, PaintEventArgs e, PictureBox Scene)
        {
            //Calculate the meter statistics
            Meter.active_meter.calculate_statistics(); 

            Image bar_texture = null;
            Player player = null;
            ImageAttributes attr = null;
            ColorMatrix cm;
            string total = "";
            string peak_value = "";
            string str_ps = "";
            string str_ps_value = "";
            string str_burst = "";

            Font font_bold = new Font(FontFamily.GenericSansSerif, 9, FontStyle.Bold);
            Font font_normal = new Font(FontFamily.GenericSansSerif, 7, FontStyle.Regular);
            SolidBrush text_brush = new SolidBrush(Color.White);

            //Calculate the usable area
            int area_width = Scene.ClientRectangle.Width; 
            int area_height = Scene.ClientRectangle.Height;
            //Bar starting offset
            int y_offset = 0; 
            int bar_height = 32;

            try
            {
                bar_texture = Properties.Resources.bar;

                //Get list of players and sort for highest damage
                foreach (KeyValuePair<string, double> pair in Meter.active_meter.get_sorted_list())
                {
                    //Get the player
                    player = (Player)Meter.active_meter.group[pair.Key];      
                    //Set the bar width based on group damage percentage
                    int bar_width = Convert.ToInt32(player.percent * area_width) / 100; // the bar width based on percentage

                    // Colorize the texture using ColorMatrix                        
                    attr = new ImageAttributes();
                    cm = get_colormatrix(player.color);
                    attr.SetColorMatrix(cm);

                    // Draw the bar
                    graphics.DrawImage(bar_texture, new Rectangle(0, y_offset, bar_width, bar_height), 0, 0, (bar_texture.Width * (float)player.percent) / 100, bar_texture.Height, GraphicsUnit.Pixel, attr);

                    //Draw the player name
                    graphics.DrawString(player.name, font_bold, text_brush, 0, y_offset);                   

                    switch (Meter.active_meter.render_mode)
                    {
                        case Meter.RENDER_MODE.render_damage:
                            peak_value = player.peak_damage.ToString();
                            str_ps = "dps";
                            str_ps_value = player.DPS.ToString("#0");
                            str_burst = player.burst_DPS.ToString("#0") + " burst";
                            total = player.damage.ToString() + " total - " + player.percent.ToString("#0.00") + "% / ";
                            break;
                        case Meter.RENDER_MODE.render_healing:
                            peak_value = player.peak_healing.ToString();
                            str_ps = "hps";
                            str_ps_value = player.HPS.ToString("#0");
                            str_burst = player.burst_HPS.ToString("#0") + " burst";  
                            total = player.healing.ToString() + " total - " + player.percent.ToString("#0.00") + "% / ";
                            break;
                    }

                    //Get the string size of damage or healing done
                    SizeF str_damge_size = graphics.MeasureString(total, font_normal); 
                    graphics.DrawString(total, font_normal, text_brush, 0, y_offset + 16);
                    peak_value += " peak";
                    graphics.DrawString(peak_value, font_normal, text_brush, str_damge_size.Width, y_offset + 16);
                    SizeF ps_size = graphics.MeasureString(str_ps, font_normal);
                    SizeF ps_value_size = graphics.MeasureString(str_ps_value, font_bold);
                    graphics.DrawString(str_ps, font_normal, text_brush, area_width - ps_size.Width, y_offset + 2);
                    graphics.DrawString(str_ps_value, font_bold, text_brush, area_width - (ps_value_size.Width + ps_size.Width), y_offset);
                    SizeF burst_size = graphics.MeasureString(str_burst, font_normal);
                    graphics.DrawString(str_burst, font_normal, text_brush, area_width - burst_size.Width, y_offset + 16);
                    //Increase offset for next bar
                    y_offset += bar_height + 1; 

                    attr.Dispose();
                    cm = null;
                }
            }
            catch (Exception exc) 
            { 
            } 
            finally
            {
                //Cleanup everything
                font_bold.Dispose();
                font_bold = null;
                font_normal.Dispose();
                font_normal = null;
                text_brush.Dispose();
                text_brush = null;
                if(bar_texture!=null)
                    bar_texture.Dispose();
                bar_texture = null;
                cm = null;
                if(attr!=null)
                    attr.Dispose();
                attr = null;
                total = null;
                peak_value = null;
                str_ps = null;
                str_ps_value = null;
                str_burst = null;
                total = null;
                GC.Collect();
            }
        }

        /// <summary>
        /// Color matrix to color depending on damage output
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static ColorMatrix get_colormatrix(Color c)
        {
            float red = (float)(c.R - Color.DarkGray.R) / 255;
            float green = (float)(c.G - Color.DarkGray.B) / 255;
            float blue = (float)(c.B - Color.DarkGray.B) / 255;
            float alpha = (float)c.A / 255;
            return new ColorMatrix(new float[][]
            {
                new float[]{1,0,0,0,0},
                new float[]{0,1,0,0,0},
                new float[]{0,0,1,0,0},
                new float[]{0,0,0,1,0},
                new float[]{red,green,blue,alpha,1}
            });
        }
    }
}
