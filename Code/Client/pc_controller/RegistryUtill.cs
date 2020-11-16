using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace pc_controller
{
    class RegistryUtill
    {
        // 프로그램의 키,어셈블리데이터를 저장할 레지스트리경로
        private const string REGISTRY_RUN_LOCATION = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        // 프로그램 실행파일이 위치하는 폴더경로
        public static String getAssemblyPath()
        {
            return Application.StartupPath;
        }


        public static void SetAutoStart(string keyName, string assemblyLocation)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_RUN_LOCATION);
            key.SetValue(keyName, assemblyLocation);
        }

        //// 자동실행
        //public static void SetAutoStart(string keyName)
        //{
        //    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
        //    {
        //        try
        //        {
        //            if (rk.GetValue(keyName) == null) 
        //            {
        //                rk.SetValue(keyName, Application.ExecutablePath.ToString());
        //            } 
        //            rk.Close();
        //        } 
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("오류: " + ex.Message.ToString());
        //        }
        //     }

        //    //// REGISTRY_RUN_LOCATION에 하위키를 생성(쓰기)하기위해 기존의 경로를 연다
        //    //RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_RUN_LOCATION);
        //    //// 하위키를 생성한다.(키,값)
        //    //key.SetValue(keyName, assemblyLocation);
        //}

        // 자동실행여부를 판단한다.
        public static bool IsAutoStartEnabled(string keyName, string assemblyLocation)
        {
            // REGISTRY_RUN_LOCATION에 저장된 값을 읽기위해 하위키를 연다.
            RegistryKey key = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_LOCATION);
            if (key == null)
                return false;
            string value = (string)key.GetValue(keyName);
            if (value == null)
                return false;
            return (value == assemblyLocation);
        }

        public static void UnSetAutoStart(string keyName)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(REGISTRY_RUN_LOCATION);
            key.DeleteValue(keyName);
        }

        //// 자동실행해제
        //public static void UnSetAutoStart(string keyName)
        //{
        //    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
        //    {
        //        try
        //        {
        //            if (rk.GetValue(keyName) != null)
        //            {
        //                rk.DeleteValue(keyName, false);
        //            }
        //            rk.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("오류: " + ex.Message.ToString());
        //        }
        //    }
        //}

        // 레지스트리 제한프로그램목록 뽑기
        public static string[] GetControlRegistryList()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
            {
                using (RegistryKey subkey = key.OpenSubKey("DisallowRun", true))
                {
                    return subkey.GetValueNames();
                }
            }
        }
        //레지스트리 제한프로그램 value 뽑기
        public static List<string> GetControlAllRegistryValue()
        {
            List<string> values = new List<string>();
            string[] list = GetControlRegistryList();
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
            {
                using (RegistryKey subkey = key.OpenSubKey("DisallowRun", true))
                {
                    foreach(string name in list)
                    {
                        values.Add(subkey.GetValue(name).ToString());
                    }
                }
            }
            return values;
        }

        // 실행가능한 프로그램 목록만들기
        public static void Create_Executable_List(List<string> programs_available)
        {
            RegistryKey rKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths");
            string[] pg_list = rKey.GetSubKeyNames();
            foreach (string str in pg_list)
            {
                if (str.Contains(".exe") || str.Contains(".EXE"))
                {
                    Console.WriteLine($"프로그램명 : {str}");
                    programs_available.Add(str);
                }
            }
        }

        //레지스트리 폴더 셋팅
        public static void CreateControlRegistry()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies"))
            {
                key.CreateSubKey("Explorer");
                using (RegistryKey subkey = key.OpenSubKey("Explorer", true))
                {
                    subkey.SetValue("DisallowRun", 1, RegistryValueKind.DWord);
                    subkey.CreateSubKey("DisallowRun");
                }
            }
        }

        //레지스트리 제어프로그램 등록
        public static void SetControlRegistryValue(string value)
        {
            string[] arr = value.Split('.');
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
            {
                using (RegistryKey subkey = key.OpenSubKey("DisallowRun", true))
                {
                    subkey.SetValue(arr[0], value);
                }
            }
        }

        //레지스트리 제어프로그램 삭제
        public static void DeleteControlRegistryValue(string name)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
            {
                using (RegistryKey subkey = key.OpenSubKey("DisallowRun", true))
                {
                    subkey.DeleteValue(name);
                }
            }
        }
        //레지스트리 제어프로그램 전체 삭제
        public static void DeleteControlAllRegistryValue(string[] names)
        {

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer"))
            {
                using (RegistryKey subkey = key.OpenSubKey("DisallowRun", true))
                {
                    for (int i = 0; i < names.Length; i++)
                        subkey.DeleteValue(names[i]);
                }
            }
        }

    }
}
