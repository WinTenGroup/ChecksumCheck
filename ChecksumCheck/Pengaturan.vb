﻿Imports System.Runtime.InteropServices
Public Class Pengaturan
    Dim AppReg As New RegEdit
    <DllImport("user32", CharSet:=CharSet.Auto, SetLastError:=True)>
    Shared Function SendMessage(
        ByVal hWnd As IntPtr,
        ByVal Msg As UInt32,
        ByVal wParam As Integer,
        ByVal lParam As IntPtr) _
        As Integer
    End Function
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Const BCM_SETSHIELD As UInt32 = &H160C
    Public CheckState As Boolean = My.Settings.ExplorerContextMenu
    Private Sub Pengaturan_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        BackColor = ColorTranslator.FromHtml("#2E2E2E")
        ChckExplorer.Checked = CheckState
        SendMessage(ChckExplorer.Handle, BCM_SETSHIELD, 0, New IntPtr(1))
        AddHandler ChckExplorer.CheckedChanged, AddressOf ChckExplorer_CheckedChanged
    End Sub

    Private Sub ChckExplorer_CheckedChanged(sender As Object, e As EventArgs)
        If Not My.User.IsInRole(ApplicationServices.BuiltInRole.Administrator) Then
            Select Case MessageBox.Show(Me, "This action require administrator permission " & Environment.NewLine & Environment.NewLine & "Would you like to restart the app and run app as administrator?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                Case DialogResult.Yes
                    Dim proc As New ProcessStartInfo
                    proc.UseShellExecute = True
                    proc.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                    proc.FileName = Application.ExecutablePath
                    proc.Verb = "runas"
                    Try
                        Process.Start(proc)
                        End
                    Catch ex As Exception

                    End Try
                Case DialogResult.No
                    Controls.Clear()
                    InitializeComponent()
                    Pengaturan_Load(e, e)
                    MsgBox("Action canceled", MsgBoxStyle.Critical)
            End Select

        Else
            If ChckExplorer.Checked = True Then
                Try
                    AppReg.CreateRegistry()
                    My.Settings.ExplorerContextMenu = True
                Catch ex As Exception
                    MsgBox("Error saat membuat context menu", MsgBoxStyle.Critical)
                End Try
            Else
                Try
                    AppReg.DeleteRegistry()
                    My.Settings.ExplorerContextMenu = False
                Catch ex As Exception
                    MsgBox("Error saat menghapus context menu", MsgBoxStyle.Critical)
                If ex.Message.Contains("not exist") Then
                    My.Settings.ExplorerContextMenu = False
                End If
                End Try
            End If
                My.Settings.Save()
        End If

    End Sub
End Class