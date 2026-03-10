Imports MySql.Data.MySqlClient
Imports System.Data

Public Class Form15

    Private dtSiswa As DataTable
    Private dvSiswa As DataView

    Private Sub Form15_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tampilData()
        AddHandler TextBoxCari.TextChanged, AddressOf TextBoxCari_TextChanged ' 🔍 aktifkan pencarian real-time
    End Sub

    ' ================== TAMPIL DATA ==================
    Public Sub tampilData()
        Try
            Call Koneksi()

            ' ============ 1. TAMPILKAN IDENTITAS USER LOGIN ============
            Label12.Text = CurrentUserNama
            Label10.Text = CurrentUserKelas

            ' ============ 2. HITUNG JUMLAH SISWA YANG DIAJAR ============
            Dim sqlJumlah As String = "SELECT COUNT(*) FROM siswa WHERE id_guru = @id_guru"
            Dim jumlahSiswa As Integer = 0

            Using cmdJumlah As New MySqlCommand(sqlJumlah, conn)
                cmdJumlah.Parameters.AddWithValue("@id_guru", CurrentUserId)
                jumlahSiswa = Convert.ToInt32(cmdJumlah.ExecuteScalar())
            End Using
            Label6.Text = jumlahSiswa.ToString()


            ' ============ 3. HITUNG RATA-RATA NILAI SISWA ============  
            Dim sqlRata As String = " SELECT AVG(rata_rata) FROM nilai WHERE id_guru = @id_guru "

            Dim rataRata As Object

            Using cmdRata As New MySqlCommand(sqlRata, conn)
                cmdRata.Parameters.AddWithValue("@id_guru", CurrentUserId)
                rataRata = cmdRata.ExecuteScalar()
            End Using

            If IsDBNull(rataRata) OrElse rataRata Is Nothing Then
                Label8.Text = "0"
            Else
                Label8.Text = Math.Round(Convert.ToDouble(rataRata), 2).ToString()
            End If

            Dim sqlData As String = "
    SELECT 
        NISN AS 'NISN',
        NAMA AS 'Nama',
        `NOMOR HP` AS 'No HP',
        `NAMA WALI` AS 'Nama Wali',
        `NOMOR WALI` AS 'No Wali',
        ALAMAT AS 'Alamat'
    FROM siswa
    WHERE id_guru = @id_guru
"

            Dim da As New MySqlDataAdapter(sqlData, conn)
            da.SelectCommand.Parameters.AddWithValue("@id_guru", CurrentUserId)

            dtSiswa = New DataTable()
            da.Fill(dtSiswa)

            dvSiswa = New DataView(dtSiswa) ' simpan ke DataView biar bisa di-filter
            DataGridView1.DataSource = dvSiswa

        Catch ex As Exception
            MessageBox.Show("Gagal menampilkan data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            conn.Close()
        End Try
    End Sub

    ' ================== 🔍 FITUR PENCARIAN SISWA ==================
    Private Sub TextBoxCari_TextChanged(sender As Object, e As EventArgs)
        If dvSiswa IsNot Nothing Then
            ' Filter berdasarkan nama siswa (kolom "Nama")
            dvSiswa.RowFilter = String.Format("[Nama] LIKE '%{0}%'", TextBoxCari.Text.Replace("'", "''"))
        End If
    End Sub

    ' ================== LOGOUT ==================
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click

        If MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ClearSession()
            Form6.TextBox1.Text = ""
            Form6.TextBox2.Text = ""
            Form6.Show()
            Me.Hide()
        End If
    End Sub


    Private Sub Label9_Click(sender As Object, e As EventArgs) Handles Label9.Click

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form14.Show()
        Me.Hide()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form16.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form11.Show()
        Me.Hide()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form3.Show()
        Me.Hide()
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub TextBoxCari_TextChanged_1(sender As Object, e As EventArgs) Handles TextBoxCari.TextChanged

    End Sub

    Private Sub Label13_Click(sender As Object, e As EventArgs) Handles Label13.Click

    End Sub
End Class
