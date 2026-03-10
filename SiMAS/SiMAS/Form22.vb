Imports MySql.Data.MySqlClient

Public Class Form22
    Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
    Dim dtSiswa As DataTable
    Dim dvSiswa As DataView
    Public Sub TampilDataSiswa()
        Try
            conn.Open()

            ' Step 1: Ambil id_guru berdasarkan kelas dari tabel user
            Dim idGuru As Integer = -1
            If ComboBox1.Text <> "" Then
                Dim cmdGuru As New MySqlCommand("SELECT id_guru FROM user WHERE kelas=@kelas", conn)
                cmdGuru.Parameters.AddWithValue("@kelas", ComboBox1.Text)
                Dim result = cmdGuru.ExecuteScalar()
                If result IsNot Nothing Then
                    idGuru = Convert.ToInt32(result)
                End If
            End If

            ' Step 2: Query siswa (semua kalau belum pilih kelas, filter kalau sudah)
            Dim sql As String
            If idGuru = -1 Then
                sql = "SELECT NISN, NAMA, `NOMOR HP` AS 'No HP', `NAMA WALI` AS 'Nama Wali', 
                              `NOMOR WALI` AS 'No Wali', ALAMAT, id_guru AS 'ID Guru' 
                       FROM siswa"
            Else
                sql = "SELECT NISN, NAMA, `NOMOR HP` AS 'No HP', `NAMA WALI` AS 'Nama Wali', 
                              `NOMOR WALI` AS 'No Wali', ALAMAT, id_guru AS 'ID Guru' 
                       FROM siswa WHERE id_guru=@id_guru"
            End If

            Dim da As New MySqlDataAdapter(sql, conn)
            If idGuru <> -1 Then da.SelectCommand.Parameters.AddWithValue("@id_guru", idGuru)

            dtSiswa = New DataTable()
            da.Fill(dtSiswa)
            dvSiswa = New DataView(dtSiswa)

            DataGridView1.DataSource = dvSiswa

            ' 🔹 Format tampilan DataGridView
            With DataGridView1
                .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                .DefaultCellStyle.Font = New Font("Segoe UI", 10)
                .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                .ColumnHeadersDefaultCellStyle.BackColor = Color.Navy
                .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
                .EnableHeadersVisualStyles = False
                .AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue
                .SelectionMode = DataGridViewSelectionMode.FullRowSelect
                .ReadOnly = True
            End With

        Catch ex As Exception
            MessageBox.Show("Gagal memuat data siswa: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If String.IsNullOrEmpty(ComboBox1.Text) Then
            MessageBox.Show("Pilih kelas terlebih dahulu sebelum menambah siswa!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Kirim nama kelas ke Form7
        Dim f As New Form7(ComboBox1.Text)
        f.ShowDialog()
        TampilDataSiswa()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Try
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Pilih satu data siswa yang ingin diubah!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim r As DataGridViewRow = DataGridView1.SelectedRows(0)

            Dim nisn As String = r.Cells("NISN").Value.ToString()
            Dim nama As String = r.Cells("NAMA").Value.ToString()
            Dim noHp As String = r.Cells("No HP").Value.ToString()
            Dim namaWali As String = r.Cells("Nama Wali").Value.ToString()
            Dim noWali As String = r.Cells("No Wali").Value.ToString()
            Dim alamat As String = r.Cells("ALAMAT").Value.ToString()  ' perhatikan alias harus sama

            Dim f As New Form8()
            f.OldNISN = nisn
            f.TextBox1.Text = nisn
            f.TextBox2.Text = nama
            f.TextBox3.Text = noHp
            f.TextBox4.Text = namaWali
            f.TextBox5.Text = noWali
            f.RichTextBox1.Text = alamat

            f.ShowDialog()
            TampilDataSiswa()

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        ' Pastikan ada baris yang dipilih
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih data siswa yang akan dihapus!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        ' Konfirmasi hapus
        Dim konfirmasi As DialogResult = MessageBox.Show(
        "Apakah Anda yakin ingin menghapus data siswa ini beserta nilai-nilainya?",
        "Konfirmasi Hapus",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    )

        If konfirmasi = DialogResult.No Then Exit Sub

        ' Ambil NISN dari baris terpilih
        Dim nisn As String = DataGridView1.SelectedRows(0).Cells("NISN").Value.ToString()

        ' Ambil id_guru dari kelas yang sedang dipilih (bukan CurrentUserId)
        Dim idGuru As Integer = -1
        Try
            Using conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
                conn.Open()
                Dim cmdGuru As New MySqlCommand("SELECT id_guru FROM user WHERE kelas=@kelas", conn)
                cmdGuru.Parameters.AddWithValue("@kelas", ComboBox1.Text)
                Dim result = cmdGuru.ExecuteScalar()
                If result IsNot Nothing Then idGuru = Convert.ToInt32(result)
            End Using
        Catch ex As Exception
            MessageBox.Show("Gagal mengambil id_guru: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End Try

        ' Jika id_guru belum ketemu, hentikan
        If idGuru = -1 Then
            MessageBox.Show("Kelas belum dipilih atau tidak ditemukan di database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        ' Proses hapus data
        Try
            Using conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
                conn.Open()

                ' Hapus nilai (jika ada)
                Dim sqlNilai As String = "DELETE FROM `nilai` WHERE `nisn`=@nisn AND `id_guru`=@id_guru"
                Using cmdNilai As New MySqlCommand(sqlNilai, conn)
                    cmdNilai.Parameters.AddWithValue("@nisn", nisn)
                    cmdNilai.Parameters.AddWithValue("@id_guru", idGuru)
                    cmdNilai.ExecuteNonQuery()
                End Using

                ' Hapus siswa
                Dim sqlSiswa As String = "DELETE FROM `siswa` WHERE `nisn`=@nisn AND `id_guru`=@id_guru"
                Using cmdSiswa As New MySqlCommand(sqlSiswa, conn)
                    cmdSiswa.Parameters.AddWithValue("@nisn", nisn)
                    cmdSiswa.Parameters.AddWithValue("@id_guru", idGuru)
                    Dim rowsDeleted As Integer = cmdSiswa.ExecuteNonQuery()

                    If rowsDeleted > 0 Then
                        MessageBox.Show("✅ Data siswa dan nilai terkait berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show("Tidak ada data siswa yang dihapus. Periksa id_guru dan NISN.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Using

            ' Refresh data di grid
            TampilDataSiswa()

        Catch ex As Exception
            MessageBox.Show("❌ Gagal menghapus data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form19.Show()
        Me.Hide()
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form17.Show()
        Me.Hide()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form4.Show()
        Me.Hide()
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ClearSession()
            Form6.TextBox1.Text = ""
            Form6.TextBox2.Text = ""
            Form6.Show()
            Me.Hide()
        End If
    End Sub

    Private Sub Form22_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TampilDataSiswa()
    End Sub


    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        TampilDataSiswa()
    End Sub

    Private Sub TextBoxCari_TextChanged_1(sender As Object, e As EventArgs) Handles TextBoxCari.TextChanged
        Try
            If dvSiswa IsNot Nothing Then
                ' Filter berdasarkan kolom "NAMA" (huruf besar kecil diabaikan)
                dvSiswa.RowFilter = String.Format("[NAMA] LIKE '%{0}%' OR [NISN] LIKE '%{0}%'", TextBoxCari.Text.Replace("'", "''"))
            End If
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat pencarian: " & ex.Message)
        End Try
    End Sub
End Class