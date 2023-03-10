using MySql.Data.MySqlClient;
using Mysqlx.Prepare;
using Mysqlx.Resultset;
using System;
using System.Data;
using System.Web;
using System.Windows.Forms;

namespace HabibStore
{
    public partial class Form1 : Form
    {
        // 
        MySqlCommand cmd;
        int i = 0;

        // koneksi ke database
        MySqlConnection koneksi = connection.getConnection();
        DataTable dataTable = new DataTable();


        // mengambil data tabe
        public void filldataTable()
        {
            using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM toko", koneksi))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;

                // retrieve the BLOB image data for each row and create an Image object
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    byte[] imageData = (byte[])row.Cells["image"].Value;
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        Image image = Image.FromStream(ms);
                        row.Cells["image"].Value = image.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                    }
                }
            }

        }

        // fungsi menghapus isi field
        public void clear()
        {
            id_barang.Clear();
            nama_barang.Clear();
            deskripsi.Clear();
            harga_beli.Clear();
            harga_jual.Clear();
            stock.Clear();
            tanggal.Value = DateTime.Now;
            imageBox.Image = null;

        }

        // mengurutkan id 
        public void resetIncrement()
        {
            MySqlScript script = new MySqlScript(koneksi, "SET @id :=0; Update toko SET id = @id := (@id+1); " +
                "ALTER TABLE toko AUTO_INCREMENT = 1;");
            script.Execute();
        }

        public Form1()
        {
            InitializeComponent();
        }

        // menampilkan data tabel dengan fungsi edit & delete
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.CurrentCell.RowIndex.ToString());
            id_barang.Text = dataGridView1.Rows[id].Cells[0].Value.ToString();
            nama_barang.Text = dataGridView1.Rows[id].Cells[1].Value.ToString(); 
            deskripsi.Text = dataGridView1.Rows[id].Cells[2].Value.ToString();
            harga_beli.Text = dataGridView1.Rows[id].Cells[3].Value.ToString();
            harga_jual.Text = dataGridView1.Rows[id].Cells[4].Value.ToString();
            stock.Text = dataGridView1.Rows[id].Cells[5].Value.ToString();
            string dateString = dataGridView1.Rows[id].Cells[6].Value.ToString();
            DateTime dateValue;
            if (DateTime.TryParse(dateString, out dateValue))
            {
                tanggal.Value = dateValue;
            }

            // retrieve the BLOB image data for the clicked row and create an Image object
            byte[] imageData = (byte[])dataGridView1.Rows[id].Cells["image"].Value;
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                Image image = Image.FromStream(ms);
                imageBox.Image = image;
                imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }

        }

        // menambah data
        private void btn_simpan_Click(object sender, EventArgs e)
        {
            // mengecek apakah ada field yang kosong
            if ((nama_barang.Text == string.Empty || deskripsi.Text == string.Empty || harga_beli.Text == string.Empty || harga_jual.Text == string.Empty || stock.Text == string.Empty))     
            {
                MessageBox.Show("Tolong Isi Semua Field!", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                // Convert image to byte array
                byte[] imageData;
                using (MemoryStream ms = new MemoryStream())
                {
                    imageBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageData = ms.ToArray();
                }
                koneksi.Open();

                resetIncrement();

                string date1 = tanggal.Value.ToString("yyyy-MM-dd");
                cmd = koneksi.CreateCommand();
                cmd.CommandText = "INSERT INTO toko(nama_barang, deskripsi, harga_beli, harga_jual, stock, tanggal_masuk, image) VALUE(@nama_barang, @deskripsi, @harga_beli, @harga_jual, @stock, @tanggal, @image)";
                cmd.Parameters.AddWithValue("@nama_barang", nama_barang.Text);
                cmd.Parameters.AddWithValue("@deskripsi", deskripsi.Text);
                cmd.Parameters.AddWithValue("@harga_beli", harga_beli.Text);
                cmd.Parameters.AddWithValue("@harga_jual", harga_jual.Text);
                cmd.Parameters.AddWithValue("@tanggal", date1);
                cmd.Parameters.AddWithValue("@stock", stock.Text);
                cmd.Parameters.AddWithValue("@image", imageData);
                
                //
                i=cmd.ExecuteNonQuery();
                if (i>0)
                {
                    MessageBox.Show("Data Berhasil Ditambah!", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Data Gagal Ditambah!", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                koneksi.Close();
                filldataTable();
                clear();
            }
        }

        // menampilkan data table yang sudah diambil
        private void Form1_Load_1(object sender, EventArgs e)
        {
            filldataTable();
           // dataGridView1.RowTemplate.Height = 25;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        // mengupdate data
        private void button2_Click(object sender, EventArgs e)
        {
            // mengecek apakah ada field yang kosong
            if ((id_barang.Text == string.Empty || nama_barang.Text == string.Empty || deskripsi.Text == string.Empty || harga_beli.Text == string.Empty || harga_jual.Text == string.Empty || stock.Text == string.Empty))
            {
                MessageBox.Show("Tolong Isi Semua Field!", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                // Convert image to byte array
                byte[] imageData;
                using (MemoryStream ms = new MemoryStream())
                {
                    imageBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageData = ms.ToArray();
                }

                koneksi.Open();
                string date1 = tanggal.Value.ToString("yyyy-MM-dd");
                cmd = koneksi.CreateCommand();
                cmd.CommandText = "UPDATE toko SET nama_barang=@nama_barang,deskripsi=@deskripsi,harga_beli=@harga_beli,harga_jual=@harga_jual,stock=@stock,tanggal_masuk=@tanggal,image=@image where id=@id";
                cmd.Parameters.AddWithValue("@id", id_barang.Text);
                cmd.Parameters.AddWithValue("@nama_barang", nama_barang.Text);
                cmd.Parameters.AddWithValue("@deskripsi", deskripsi.Text);
                cmd.Parameters.AddWithValue("@harga_beli", harga_beli.Text);
                cmd.Parameters.AddWithValue("@harga_jual", harga_jual.Text);
                cmd.Parameters.AddWithValue("@stock", stock.Text);
                cmd.Parameters.AddWithValue("@tanggal", date1);
                cmd.Parameters.AddWithValue("@image", imageData);
                cmd.ExecuteNonQuery();
                //
                i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    MessageBox.Show("Data Berhasil Diupdate!", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Data Gagal Diupdate!", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                koneksi.Close();
                filldataTable();
                clear();
            }
        }

        // menghapus data
        private void btn_delete_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(dataGridView1.CurrentCell.RowIndex.ToString());

            koneksi.Open();
            cmd = koneksi.CreateCommand();
            cmd.CommandText = "DELETE FROM `toko` WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", id_barang.Text);
            i = cmd.ExecuteNonQuery();
            if (i > 0)
            {
                MessageBox.Show("Data Berhasil Dihapus!", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Data Gagal Dihapus!", "CRUD", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            koneksi.Close();
            resetIncrement();
            filldataTable();
        }

        // menghapus field
        private void btn_clear_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void tanggal_ValueChanged(object sender, EventArgs e)
        {

        }

        //mencari data
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM toko WHERE id LIKE @search OR nama_barang LIKE @search OR deskripsi LIKE @search OR harga_beli LIKE @search OR harga_jual LIKE @search OR stock LIKE @search OR tanggal_masuk LIKE @search", koneksi))
            {
                da.SelectCommand.Parameters.AddWithValue("@search", "%" + txt_search.Text + "%");

                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                // resize the image for each row
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    byte[] imageData = (byte[])row.Cells["image"].Value;
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        Image image = Image.FromStream(ms);
                        row.Cells["image"].Value = image.GetThumbnailImage(150, 150, null, IntPtr.Zero);
                    }
                }
            }

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void btn_select_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfd = new OpenFileDialog();
            openfd.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            if (openfd.ShowDialog() == DialogResult.OK)
            {
                imageBox.Image = new Bitmap(openfd.FileName);
                imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }
    }
}       