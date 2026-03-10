Imports MySql.Data.MySqlClient
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports System.IO

Public Class Form3
    Public Sub RefreshData()
        tampilDataGrid()
        tampilRekap()
    End Sub

    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        tampilDataGrid()
        tampilRekap()
    End Sub

    ' ================= TAMPILKAN DATA GRID =================
    Private Sub tampilDataGrid()
        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()

            Dim sql As String = "SELECT `NISN`, `NAMA`,
                                        `NILAI UH` AS 'NILAI UH',
                                        `NILAI UTS` AS 'NILAI UTS',
                                        `NILAI UAS` AS 'NILAI UAS',
                                        `NILAI KEAKTIFAN` AS 'NILAI KEAKTIFAN',
                                        `RATA RATA` AS 'RATA RATA'
                                 FROM nilai
                                 WHERE id_guru=@id_guru"

            Dim da As New MySqlDataAdapter(sql, conn)
            da.SelectCommand.Parameters.AddWithValue("@id_guru", CurrentUserId)

            Dim ds As New DataSet()
            da.Fill(ds, "nilai")
            DataGridView1.DataSource = ds.Tables("nilai")

        Catch ex As Exception
            MessageBox.Show("Gagal ambil data nilai: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' ================= TAMPILKAN REKAP LABEL =================
    Private Sub tampilRekap()
        Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
        Try
            conn.Open()

            Dim sql As String = "SELECT COUNT(*) AS jumlah,
                                        AVG(`RATA RATA`) AS rata,
                                        MAX(`RATA RATA`) AS tertinggi,
                                        MIN(`RATA RATA`) AS terendah
                                 FROM nilai
                                 WHERE id_guru=@id_guru"

            Dim cmd As New MySqlCommand(sql, conn)
            cmd.Parameters.AddWithValue("@id_guru", CurrentUserId)

            Dim rd As MySqlDataReader = cmd.ExecuteReader()

            If rd.Read() Then
                Label5.Text = CurrentUserKelas
                Label2.Text = rd("jumlah").ToString()
                Label7.Text = If(IsDBNull(rd("rata")), "0", FormatNumber(rd("rata"), 2))
                Label9.Text = If(IsDBNull(rd("tertinggi")), "0", rd("tertinggi").ToString())
                Label11.Text = If(IsDBNull(rd("terendah")), "0", rd("terendah").ToString())
            End If
            rd.Close()

        Catch ex As Exception
            MessageBox.Show("Gagal ambil rekap: " & ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    ' ================= EKSPOR PDF (BUTTON 8) =================
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Try
            ' Pastikan ada baris yang dipilih
            If DataGridView1.SelectedRows.Count = 0 Then
                MessageBox.Show("Pilih satu siswa terlebih dahulu dari tabel.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Ambil data dari baris yang dipilih
            Dim selectedRow As DataGridViewRow = DataGridView1.SelectedRows(0)
            Dim nisn As String = selectedRow.Cells("NISN").Value.ToString()
            Dim nama As String = selectedRow.Cells("NAMA").Value.ToString()
            Dim nilaiUH As String = selectedRow.Cells("NILAI UH").Value.ToString()
            Dim nilaiUTS As String = selectedRow.Cells("NILAI UTS").Value.ToString()
            Dim nilaiUAS As String = selectedRow.Cells("NILAI UAS").Value.ToString()
            Dim nilaiKeaktifan As String = selectedRow.Cells("NILAI KEAKTIFAN").Value.ToString()

            ' Simpan file PDF
            Dim sfd As New SaveFileDialog()
            sfd.Filter = "PDF Files (*.pdf)|*.pdf"
            sfd.FileName = "Raport_" & nama & ".pdf"

            If sfd.ShowDialog() = DialogResult.OK Then
                Dim savePath As String = sfd.FileName
                Dim conn As New MySqlConnection("server=localhost;user id=root;password=;database=simas")
                conn.Open()

                ' Buat dokumen PDF
                Dim doc As New Document(PageSize.A4, 40, 40, 40, 40)
                PdfWriter.GetInstance(doc, New FileStream(savePath, FileMode.Create))
                doc.Open()

                ' === Header Sekolah ===
                Dim header As New Paragraph("DINAS PENDIDIKAN KOTA SIDOARJO", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12))
                header.Alignment = Element.ALIGN_CENTER
                doc.Add(header)
                Dim subheader As New Paragraph("LAPORAN HASIL CAPAIAN KOMPETENSI PESERTA DIDIK", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12))
                subheader.Alignment = Element.ALIGN_CENTER
                doc.Add(subheader)
                Dim subheader2 As New Paragraph("SDN GENTING 1 KABUPATEN SIDOARJO", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12))
                subheader2.Alignment = Element.ALIGN_CENTER
                doc.Add(subheader2)
                doc.Add(New Paragraph(" "))
                doc.Add(New Paragraph(" "))

                ' === Identitas Siswa ===
                Dim fontIsi As Font = FontFactory.GetFont(FontFactory.HELVETICA, 11)
                doc.Add(New Paragraph("Nama                  : " & nama, fontIsi))
                doc.Add(New Paragraph("NISN                   : " & nisn, fontIsi))
                doc.Add(New Paragraph("Kelas                   : " & Label5.Text, fontIsi))
                doc.Add(New Paragraph("Semester             : 1 (Ganjil)", fontIsi))
                doc.Add(New Paragraph("Tahun Pelajaran  : 2025/2026", fontIsi))
                doc.Add(New Paragraph(" "))

                ' === Tabel Nilai ===
                Dim nilaiTable As New PdfPTable(5)
                nilaiTable.WidthPercentage = 100

                Dim headers() As String = {"Nilai UH", "Nilai UTS", "Nilai UAS", "Nilai Keaktifan", "Rata-rata"}
                For Each h As String In headers
                    Dim cell As New PdfPCell(New Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY
                    cell.HorizontalAlignment = Element.ALIGN_CENTER
                    nilaiTable.AddCell(cell)
                Next

                Dim rata2 As Double = (Val(nilaiUH) + Val(nilaiUTS) + Val(nilaiUAS) + Val(nilaiKeaktifan)) / 4
                Dim nilai() As String = {nilaiUH, nilaiUTS, nilaiUAS, nilaiKeaktifan, rata2.ToString("0.00")}
                For Each n In nilai
                    Dim c As New PdfPCell(New Phrase(n))
                    c.HorizontalAlignment = Element.ALIGN_CENTER
                    nilaiTable.AddCell(c)
                Next

                doc.Add(nilaiTable)
                doc.Add(New Paragraph(" "))

                ' === Tambahkan Rekap Absensi ===
                Dim sqlAbsensi As String = "
                    SELECT sakit, izin, alpha, 
                           (60 - (sakit + izin + alpha)) AS total_hadir
                    FROM rekap_absensi 
                    WHERE nisn=@nisn"
                Dim cmd As New MySqlCommand(sqlAbsensi, conn)
                cmd.Parameters.AddWithValue("@nisn", nisn)

                Dim rd As MySqlDataReader = cmd.ExecuteReader()
                If rd.Read() Then
                    doc.Add(New Paragraph("Rekap Absensi Siswa", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                    doc.Add(New Paragraph(" "))

                    Dim absensiTable As New PdfPTable(4)
                    absensiTable.WidthPercentage = 100

                    Dim headersAbs() As String = {"Sakit", "Izin", "Alpha", "Total Hadir"}
                    For Each h As String In headersAbs
                        Dim cell As New PdfPCell(New Phrase(h, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                        cell.BackgroundColor = BaseColor.LIGHT_GRAY
                        cell.HorizontalAlignment = Element.ALIGN_CENTER
                        absensiTable.AddCell(cell)
                    Next

                    absensiTable.AddCell(rd("sakit").ToString())
                    absensiTable.AddCell(rd("izin").ToString())
                    absensiTable.AddCell(rd("alpha").ToString())
                    absensiTable.AddCell(rd("total_hadir").ToString())

                    doc.Add(absensiTable)
                    doc.Add(New Paragraph(" "))
                Else
                    doc.Add(New Paragraph("Data absensi belum tersedia untuk siswa ini.", FontFactory.GetFont(FontFactory.HELVETICA, 10)))
                    doc.Add(New Paragraph(" "))
                End If
                rd.Close()

                ' === Deskripsi Sikap ===
                doc.Add(New Paragraph("Deskripsi Sikap: Siswa menunjukkan semangat belajar tinggi dan aktif dalam pembelajaran.", FontFactory.GetFont(FontFactory.HELVETICA, 10)))
                doc.Add(New Paragraph(" "))

                ' === Tanda Tangan ===
                doc.Add(New Paragraph("Surabaya, " & DateTime.Now.ToString("dd MMMM yyyy")))
                doc.Add(New Paragraph("Guru Mata Pelajaran,", FontFactory.GetFont(FontFactory.HELVETICA, 10)))
                doc.Add(New Paragraph(" "))
                doc.Add(New Paragraph(" "))
                doc.Add(New Paragraph(CurrentUserNama, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                doc.Add(New Paragraph(" "))

                doc.Close()
                conn.Close()

                MessageBox.Show("Raport siswa berhasil diekspor ke PDF: " & savePath, "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Process.Start(savePath)
            End If

        Catch ex As Exception
            MessageBox.Show("Gagal ekspor PDF: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button4_Click_1(sender As Object, e As EventArgs) Handles Button4.Click
        Form4.Show()
        Me.Hide()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.Show()
        Me.Hide()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form2.Show()
        Me.Hide()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Form11.Show()
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
End Class
