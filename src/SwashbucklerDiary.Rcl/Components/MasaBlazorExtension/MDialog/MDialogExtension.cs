﻿using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Components
{
    //The significance of extension is to enable dialog or similar components to support back button
    public class MDialogExtension : MDialog
    {
        private bool firstOpen = true;

        [Inject]
        protected INavigateController NavigateController { get; set; } = default!;

        [Parameter]
        public bool MyValue
        {
            get => base.Value;
            set => SetValue(value);
        }

        [Parameter]
        public EventCallback<bool> MyValueChanged
        {
            get => base.ValueChanged;
            set => base.ValueChanged = value;
        }

        [Parameter]
        public EventCallback<bool> OnAfterShowContent { get; set; }

        [Parameter]
        public EventCallback OnBeforeShowContent { get; set; }

        [Parameter]
        public bool OnMousedownPreventDefault { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AfterShowContent = HandleOnAfterShowContent;
            BeforeShowContent = HandleOnBeforeShowContent;
        }

        protected override ValueTask DisposeAsyncCore()
        {
            if (Value)
            {
                NavigateController.RemoveHistoryAction(Close);
            }

            return base.DisposeAsyncCore();
        }

        private static readonly PropertyInfo overlayRefProperty = typeof(MDialog).GetProperty("OverlayRef", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?? throw new Exception("Property OverlayRef does not exist");

        private void SetValue(bool value)
        {
            if (base.Value == value)
            {
                return;
            }

            base.Value = value;
            if (value)
            {
                NavigateController.AddHistoryAction(Close);
            }
            else
            {
                NavigateController.RemoveHistoryAction(Close);
            }
        }

        private async void Close()
        {
            MyValue = false;
            if (MyValueChanged.HasDelegate)
            {
                await MyValueChanged.InvokeAsync(false);
            }
        }

        private async Task HandleOnAfterShowContent(bool value)
        {
            if (firstOpen)
            {
                firstOpen = false;
                if (OnMousedownPreventDefault)
                {
                    ElementReference? overlayRef = overlayRefProperty.GetValue(this) as ElementReference?;

                    List<object> args = [ContentRef, overlayRef];
                    await Js.InvokeVoidAsync("preventDefaultOnmousedown", args);
                }
            }

            if (OnAfterShowContent.HasDelegate)
            {
                await OnAfterShowContent.InvokeAsync(value);
            }
        }

        private async Task HandleOnBeforeShowContent()
        {
            if (OnBeforeShowContent.HasDelegate)
            {
                await OnBeforeShowContent.InvokeAsync();
            }
        }
    }
}
