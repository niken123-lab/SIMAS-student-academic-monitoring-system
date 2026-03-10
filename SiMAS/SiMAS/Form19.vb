Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports MySql.Data.MySqlClient

Public Class Form19
    Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
    Dim dtGuru As DataTable
    Dim dvGuru As DataView


    Private Sub TampilDataWaliKelas()
        Try
            conn.Open()

            Dim query As String = "
                SELECT 
                    id_guru AS 'ID Guru', 
                    nama AS 'Nama Wali Kelas', 
                    kelas AS 'Kelas', 
                    email AS 'Email', 
                    password AS 'Password'
                FROM user"
            Dim adapter As New MySqlDataAdapter(query, conn)

            dtGuru = New DataTable()
            adapter.Fill(dtGuru)
            dvGuru = New DataView(dtGuru)

            DataGridView1.DataSource = dvGuru

            ' 🔹 Format tampilan tabel
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
            MessageBox.Show("Gagal memuat data wali kelas: " & ex.Message)
        Finally
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.Show()
        Me.Hide()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form22.Show()
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

    Private Sub Form19_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TampilDataWaliKelas()
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        ' → BUKA FORM INPUT KOSONG (bukan dari row yg dipilih)
        Dim f As New Form20()
        f.Text = "Input Data Wali Kelas"

        ' Kosongkan field (kalau form menyimpan nilai sebelumnya)

        If f.Controls.ContainsKey("TextBox2") Then f.TextBox1.Text = "" ' nama
        If f.Controls.ContainsKey("TextBox3") Then f.TextBox2.Text = "" ' kelas
        If f.Controls.ContainsKey("TextBox4") Then f.TextBox3.Text = "" ' email
        If f.Controls.ContainsKey("TextBox5") Then f.TextBox4.Text = "" ' password (jika ada)

        f.ShowDialog()

        ' Refresh tabel setelah selesai input
        TampilDataWaliKelas()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Try
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Pilih satu data wali kelas yang ingin diubah!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)

            ' Ambil data
            Dim id_guru As Integer = Convert.ToInt32(selectedRow.Cells("ID Guru").Value)
            Dim nama As String = selectedRow.Cells("Nama Wali Kelas").Value.ToString()
            Dim kelas As String = selectedRow.Cells("Kelas").Value.ToString()
            Dim email As String = selectedRow.Cells("Email").Value.ToString()
            Dim password As String = selectedRow.Cells("Password").Value.ToString()

            ' Kirim data ke Form21
            Dim f As New Form21()
            f.IdGuru = id_guru
            f.TextBox1.Text = nama
            f.TextBox2.Text = kelas
            f.TextBox3.Text = email
            f.TextBox4.Text = password

            f.ShowDialog()

            ' Refresh tabel setelah ditutup
            TampilDataWaliKelas()

        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub TextBoxCari_TextChanged(sender As Object, e As EventArgs) Handles TextBoxCari.TextChanged
        Try
            If dvGuru IsNot Nothing Then
                ' Filter berdasarkan kolom "NAMA" (huruf besar kecil diabaikan)
                dvGuru.RowFilter = String.Format("[Nama Wali Kelas] LIKE '%{0}%'", TextBoxCari.Text.Replace("'", "''"))
            End If
        Catch ex As Exception
            MessageBox.Show("Terjadi kesalahan saat pencarian: " & ex.Message)
        End Try
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Try
            ' Pastikan ada baris yang dipilih
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Pilih satu data wali kelas yang ingin dihapus!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            ' Ambil baris yang dipilih
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim id_guru As String = selectedRow.Cells("ID Guru").Value.ToString()
            Dim nama As String = selectedRow.Cells("Nama Wali Kelas").Value.ToString()

            ' Konfirmasi penghapusan
            Dim confirm = MessageBox.Show(
            $"Apakah Anda yakin ingin menghapus data wali kelas '{nama}' (ID: {id_guru})?",
            "Konfirmasi Hapus",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        )

            If confirm = DialogResult.No Then
                Return
            End If

            ' Eksekusi penghapusan
            Using conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
                conn.Open()
                Dim sql As String = "DELETE FROM `user` WHERE `id_guru` = @id_guru;"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id_guru", id_guru)
                    Dim rows = cmd.ExecuteNonQuery()

                    If rows > 0 Then
                        MessageBox.Show("Data wali kelas berhasil dihapus!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Else
                        MessageBox.Show("Data tidak ditemukan atau sudah dihapus sebelumnya.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    End If
                End Using
            End Using

            ' Refresh tampilan tabel setelah hapus
            TampilDataWaliKelas()

        Catch ex As MySqlException
            MessageBox.Show("Gagal menghapus data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class