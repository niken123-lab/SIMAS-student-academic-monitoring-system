Imports MySql.Data.MySqlClient

Public Class Form2

    Public Sub RefreshData()
        tampilNilai()


    End Sub
    '================= NAVIGASI MENU =================
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form3.RefreshData()
        Form3.Show()
        Me.Hide()

    End Sub



    '================= INPUT NILAI (Form9) =================
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih siswa terlebih dahulu untuk input nilai")
            Exit Sub
        End If

        Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim nisn As String = row.Cells("NISN").Value.ToString()
        Dim nama As String = row.Cells("NAMA").Value.ToString()

        ' Kirim ke Form9
        Dim frm As New Form9(nisn, nama)
        frm.ShowDialog()

        ' Refresh data setelah input
        tampilNilai()
    End Sub

    '================= LOAD FORM =================
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label5.Text = CurrentUserKelas
        tampilNilai()
    End Sub

    '================= TAMPILKAN NILAI =================
    Public Sub tampilNilai()
        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()

            ' Sinkron siswa baru ke tabel nilai (khusus milik guru login)
            Dim syncSql As String = "INSERT INTO nilai (NISN, NAMA, id_guru) " &
                        "SELECT s.NISN, s.NAMA, s.id_guru FROM siswa s " &
                        "WHERE s.id_guru=@id_guru " &
                        "AND NOT EXISTS (SELECT 1 FROM nilai n WHERE n.NISN = s.NISN AND n.id_guru=@id_guru)"

            Dim syncCmd As New MySqlCommand(syncSql, conn)
            syncCmd.Parameters.AddWithValue("@id_guru", CurrentUserId)
            syncCmd.ExecuteNonQuery()

            ' Tampilkan nilai hanya milik guru login
            Dim sql As String = "SELECT " &
                                "`NISN` AS 'NISN', " &
                                "`NAMA` AS 'NAMA', " &
                                "`NILAI UH` AS 'NILAI UH', " &
                                "`NILAI UTS` AS 'NILAI UTS', " &
                                "`NILAI UAS` AS 'NILAI UAS', " &
                                "`NILAI KEAKTIFAN` AS 'NILAI KEAKTIFAN', " &
                                "`RATA RATA` AS 'RATA RATA' " &
                                "FROM nilai WHERE id_guru=@id_guru"

            Dim da As New MySqlDataAdapter(sql, conn)
            da.SelectCommand.Parameters.AddWithValue("@id_guru", CurrentUserId)

            Dim ds As New DataSet()
            da.Fill(ds, "nilai")
            DataGridView1.DataSource = ds.Tables("nilai")

        Catch ex As Exception
            MessageBox.Show("Gagal mengambil data nilai: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    '================= HAPUS DATA =================
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih siswa yang mau dihapus nilainya!")
            Exit Sub
        End If

        Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim nisn As String = row.Cells("NISN").Value.ToString()

        ' Konfirmasi hapus
        Dim result As DialogResult = MessageBox.Show("Apakah Anda yakin ingin menghapus nilai siswa dengan NISN: " & nisn & " ?",
                                                 "Konfirmasi Hapus", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.No Then Exit Sub

        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()
            Dim sql As String = "DELETE FROM nilai WHERE NISN=@nisn AND id_guru=@id_guru"
            Dim cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@nisn", nisn)
            cmd.Parameters.AddWithValue("@id_guru", CurrentUserId)

            cmd.ExecuteNonQuery()
            MessageBox.Show("Data nilai berhasil dihapus!")

            ' Refresh DataGridView
            tampilNilai()
        Catch ex As Exception
            MessageBox.Show("Error hapus nilai: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    '================= UBAH NILAI (Form10) =================
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If DataGridView1.SelectedRows.Count = 0 Then
            MessageBox.Show("Pilih siswa yang mau diubah nilainya!")
            Exit Sub
        End If

        Dim row As DataGridViewRow = DataGridView1.SelectedRows(0)
        Dim nisn As String = row.Cells("NISN").Value.ToString()
        Dim nama As String = row.Cells("NAMA").Value.ToString()
        Dim uh As String = row.Cells("NILAI UH").Value.ToString()
        Dim uts As String = row.Cells("NILAI UTS").Value.ToString()
        Dim uas As String = row.Cells("NILAI UAS").Value.ToString()
        Dim aktif As String = row.Cells("NILAI KEAKTIFAN").Value.ToString()

        ' Kirim data ke Form10
        Dim frm As New Form10(nisn, nama, uh, uts, uas, aktif)
        frm.ShowDialog()

        ' Refresh data setelah ubah
        tampilNilai()
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Form1.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form11.Show()
        Me.Hide()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Form4.Show()
        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            ClearSession()
            Form6.TextBox1.Text = ""
            Form6.TextBox2.Text = ""
            Form6.Show()
            Me.Hide()
        End If


    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Label11_Click(sender As Object, e As EventArgs) Handles Label11.Click

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click

    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

    End Sub
End Class