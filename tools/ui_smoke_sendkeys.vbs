Set sh = CreateObject("WScript.Shell")
sh.Run """c:\MainBase\Babylon Archive Core Raws\BabylonArchiveCore.Desktop\bin\Debug\net10.0-windows\BabylonArchiveCore.Desktop.exe""", 1, false
WScript.Sleep 3000
sh.AppActivate "Babylon Archive Core - Session 10 Gameplay Harness"
WScript.Sleep 400
sh.SendKeys "{ESC}"
WScript.Sleep 500
sh.SendKeys "p"
WScript.Sleep 500
sh.SendKeys "{ESC}"
WScript.Sleep 2000
