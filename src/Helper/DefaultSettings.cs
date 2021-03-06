﻿using Microsoft.Win32;
using System;
using System.Linq;

namespace Helper
{
    internal class DefaultSettings
    {
        private string _textLogo;

        #region Internal properties

        internal string DomainName { get; private set; }
        internal string Server { get; private set; }
        internal int Port { get; private set; }
        internal string MailName { get; private set; }
        internal string TextLogo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_textLogo))
                    return "TEXT LOGO";
                else
                    return _textLogo;
            }
        }

        internal string CurrentUserName { get; }

        internal string MailFrom { get => CurrentUserName + "@" + DomainName; }
        internal string MailTo { get => MailName + "@" + DomainName; }

        #endregion

        private readonly string _prefixRegistrySoftware;
        private readonly string _prefixKeyApplication;
        private readonly string _prefixRegistryKey;

        private RegistryKey _registryKeyApplication;
        private RegistryKey _registryKeyApplicationValues;

        internal DefaultSettings(bool getSettings = false)
        {
            _prefixRegistrySoftware = "Software";
            _prefixKeyApplication = "Helper";
            _prefixRegistryKey = _prefixRegistrySoftware + "\\" + _prefixKeyApplication;

            CurrentUserName = Environment.UserName;

            if (getSettings)
            {
                try
                {
                    GetDefaultSettings();
                }
                catch (Exception)
                {
                    SetDefaultSettings();
                }
            }
        }

        internal void GetDefaultSettings()
        {
            try
            {
                using (RegistryKey currentUser = Registry.CurrentUser)
                {
                    using (RegistryKey registryKeyApplication = currentUser.OpenSubKey(_prefixRegistryKey))
                    {
                        _registryKeyApplication = registryKeyApplication;

                        Server = GetValue("server");
                        Port = int.Parse(GetValue("port"));
                        DomainName = GetValue("domainname");
                        MailName = GetValue("mailname");
                        _textLogo = GetValue("textlogo");
                    }
                }

                if (string.IsNullOrWhiteSpace(Server)
                    || Port == 0
                    || string.IsNullOrWhiteSpace(DomainName)
                    || string.IsNullOrWhiteSpace(MailName))
                    throw new Exception("Не заполнен один или несколько параметров.");
            }
            catch (Exception)
            {
                throw new Exception("Не удалось получить настройки. Обратитесь к администратору.");
            }
        }

        internal void SetDefaultSettings()
        {
            using (RegistryKey currentUser = Registry.CurrentUser)
            {
                using (RegistryKey registryKeyApplication = currentUser.OpenSubKey(_prefixRegistrySoftware, true))
                {
                    if (registryKeyApplication == null)
                        _registryKeyApplication = currentUser.CreateSubKey(_prefixRegistrySoftware);
                    else
                        _registryKeyApplication = registryKeyApplication;

                    using (RegistryKey registryKeyApplicationValues = registryKeyApplication.OpenSubKey(_prefixRegistryKey, true))
                    {
                        if (registryKeyApplicationValues == null)
                            _registryKeyApplicationValues = registryKeyApplication.CreateSubKey(_prefixKeyApplication);
                        else
                            _registryKeyApplicationValues = registryKeyApplicationValues;

                        string[] names = _registryKeyApplicationValues.GetValueNames();

                        SetValueIfNotFinded(names, "server");
                        SetValueIfNotFinded(names, "port");
                        SetValueIfNotFinded(names, "domainname");
                        SetValueIfNotFinded(names, "mailname");
                        SetValueIfNotFinded(names, "textlogo");
                    }
                }
            }
        }

        private string GetValue(string keyName) => _registryKeyApplication.GetValue(keyName).ToString();

        private void SetValueIfNotFinded(string[] names, string key, string value = "")
        {
            if (string.IsNullOrWhiteSpace(names.FirstOrDefault(f => f == key)))
                SetValue(key, value);
        }

        private void SetValue(string key, string value)
        {
            _registryKeyApplicationValues.SetValue(key, value);
        }
    }
}
