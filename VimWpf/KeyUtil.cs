﻿using System.Windows.Input;

namespace Vim.UI.Wpf
{
    public static class KeyUtil
    {
        private static KeyboardMap _keyboardMap;

        private static KeyboardMap GetOrCreateKeyboardMap()
        {
            var keyboardId = NativeMethods.GetKeyboardLayout(0);
            if (_keyboardMap == null || _keyboardMap.KeyboardId != keyboardId)
            {
                _keyboardMap = new KeyboardMap(keyboardId);
            }

            return _keyboardMap;
        }

        public static KeyModifiers ConvertToKeyModifiers(ModifierKeys keys)
        {
            return KeyboardMap.ConvertToKeyModifiers(keys);
        }

        /// <summary>
        /// Is this a known dead key
        /// </summary>
        public static bool IsDeadKey(Key key)
        {
            return GetOrCreateKeyboardMap().IsDeadKey(key, ModifierKeys.None);
        }

        /// <summary>
        /// Is this the AltGr key combination.  This is not directly representable in WPF
        /// logic but the best that can be done is to check for Alt + Control
        /// </summary>
        public static bool IsAltGr(ModifierKeys modifierKeys)
        {
            return modifierKeys == (ModifierKeys.Alt | ModifierKeys.Control);
        }

        /// <summary>
        /// When the user is typing we get events for every single key press.  This means that 
        /// typing something like an upper case character will cause at least 2 events to be
        /// generated.  
        ///  1) LeftShift 
        ///  2) LeftShift + b
        /// This helps us filter out items like #1 which we don't want to process
        /// </summary>
        public static bool IsNonInputKey(Key k)
        {
            switch (k)
            {
                case Key.LeftAlt:
                case Key.LeftCtrl:
                case Key.LeftShift:
                case Key.RightAlt:
                case Key.RightCtrl:
                case Key.RightShift:
                case Key.System:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsInputKey(Key k)
        {
            return !IsNonInputKey(k);
        }

        public static KeyInput CharAndModifiersToKeyInput(char c, ModifierKeys modifierKeys)
        {
            return GetOrCreateKeyboardMap().GetKeyInput(c, IsAltGr(modifierKeys) ? ModifierKeys.None : modifierKeys);
        }

        public static bool TryConvertToKeyInput(Key key, out KeyInput keyInput)
        {
            return GetOrCreateKeyboardMap().TryGetKeyInput(key, out keyInput);
        }

        public static bool TryConvertToKeyInput(Key key, ModifierKeys modifierKeys, out KeyInput keyInput)
        {
            return GetOrCreateKeyboardMap().TryGetKeyInput(key, modifierKeys, out keyInput);
        }

        /// <summary>
        /// Try and convert the VimKey to a WPF key.  Note this is not a lossy conversion.  If any
        /// ModifierKeys would be necessary to produce the VimKey then the conversion will fail
        /// </summary>
        public static bool TryConvertToKeyOnly(VimKey vimKey, out Key key)
        {
            ModifierKeys modifierKeys;
            return TryConvertToKey(vimKey, out key, out modifierKeys) && modifierKeys == ModifierKeys.None;
        }

        /// <summary>
        /// Try and convert the VimKey to a WPF key.  Note this is a lossy conversion as modifiers
        /// will be dropped.  So for example UpperN will still map to Key.N but the fidelity of
        /// shift will be lost
        /// </summary>
        public static bool TryConvertToKey(VimKey vimKey, out Key key)
        {
            ModifierKeys modifierKeys;
            return TryConvertToKey(vimKey, out key, out modifierKeys);
        }

        /// <summary>
        /// Try and convert the VimKey to a WPF key and the associated modifiers
        /// </summary>
        public static bool TryConvertToKey(VimKey vimKey, out Key key, out ModifierKeys modifierKeys)
        {
            return GetOrCreateKeyboardMap().TryGetKey(vimKey, out key, out modifierKeys);
        }
    }
}
