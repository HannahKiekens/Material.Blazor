﻿using Material.Blazor.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Material.Blazor
{
    /// <summary>
    /// A Material Theme segmented button orientated as a multi-select.
    /// </summary>
    public partial class MBSegmentedButtonMulti<TItem> : MultiSelectComponent<TItem, MBIconBearingSelectElement<TItem>>
    {
        /// <summary>
        /// If this component is rendered inside a single-select segmented button, add the "" class.
        /// </summary>
        [CascadingParameter] private MBSegmentedButtonSingle<TItem> SegmentedButtonSingle { get; set; }


        private MBIconBearingSelectElement<TItem>[] ItemsArray { get; set; }
        private bool IsSingleSelect { get; set; }
        private IDisposable ObjectReference { get; set; }
        private string GroupRole => (SegmentedButtonSingle == null) ? "group" : "radiogroup";
        private Dictionary<string, object>[] SegmentAttributes { get; set; }
        private ElementReference SegmentedButtonReference { get; set; }


        // Would like to use <inheritdoc/> however DocFX cannot resolve to references outside Material.Blazor
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            IsSingleSelect = SegmentedButtonSingle != null;

            ClassMapperInstance
                .Add("mdc-segmented-button")
                .AddIf("mdc-segmented-button--single-select", () => IsSingleSelect);

            ItemsArray = Items.ToArray();

            SegmentAttributes = new Dictionary<string, object>[ItemsArray.Length];

            for (int i = 0; i < ItemsArray.Length; i++)
            {
                SegmentAttributes[i] = new();

                var selected = Value.Contains(ItemsArray[i].SelectedValue);

                SegmentAttributes[i].Add("class", "mdc-segmented-button__segment mdc-segmented-button--touch" + (selected ? " mdc-segmented-button__segment--selected" : ""));

                if (IsSingleSelect)
                {
                    SegmentAttributes[i].Add("role", "radio");
                    SegmentAttributes[i].Add("aria-checked", selected.ToString().ToLower());
                }
                else
                {
                    SegmentAttributes[i].Add("aria-pressed", selected.ToString().ToLower());
                }
            }

            SetComponentValue += OnValueSetCallback;
            OnDisabledSet += OnDisabledSetCallback;

            ObjectReference = DotNetObjectReference.Create(this);
        }


        private bool _disposed = false;
        protected override async ValueTask DisposeAsync(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                ObjectReference?.Dispose();
            }

            _disposed = true;

            await base.DisposeAsync(disposing);
        }


        /// <summary>
        /// For Material Theme to notify of menu item selection via JS Interop.
        /// </summary>
        [JSInvokable("NotifyMultiSelectedAsync")]
        public async Task NotifyMultiSelectedAsync(bool[] selected)
        {
            var selectedIndexes = Enumerable.Range(0, selected.Length).Where(i => selected[i]);
            ComponentValue = ItemsArray.Where((item, index) => selectedIndexes.Contains(index)).Select(x => x.SelectedValue).ToArray();
            await Task.CompletedTask;
        }


        /// <summary>
        /// For Material Theme to notify of menu item selection via JS Interop.
        /// </summary>
        [JSInvokable("NotifySingleSelectedAsync")]
        public async Task NotifySingleSelectedAsync(int index)
        {
            ComponentValue = new TItem[] { ItemsArray[index].SelectedValue };
            await Task.CompletedTask;
        }


        /// <summary>
        /// Callback for value the value setter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnValueSetCallback(object sender, EventArgs e) => InvokeAsync(async () => await JsRuntime.InvokeVoidAsync("MaterialBlazor.MBSegmentedButtonMulti.setSelected", SegmentedButtonReference, Items.Select(x => Value.Contains(x.SelectedValue)).ToArray()));


        /// <summary>
        /// Callback for value the Disabled value setter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnDisabledSetCallback(object sender, EventArgs e) => InvokeAsync(async () => await JsRuntime.InvokeVoidAsync("MaterialBlazor.MBSegmentedButtonMulti.setDisabled", SegmentedButtonReference, AppliedDisabled));


        /// <inheritdoc/>
        private protected override async Task InstantiateMcwComponent() => await JsRuntime.InvokeVoidAsync("MaterialBlazor.MBSegmentedButtonMulti.init", SegmentedButtonReference, IsSingleSelect, ObjectReference);


        /// <inheritdoc/>
        private protected override async Task DestroyMcwComponent() => await JsRuntime.InvokeVoidAsync("MaterialBlazor.MBSegmentedButtonMulti.destroy", SegmentedButtonReference);


        /// <summary>
        /// Used by <see cref="MBSegmentedButtonSingle{TItem}"/> to set the value.
        /// </summary>
        /// <param name="value"></param>
        internal void SetSingleSelectValue(TItem value)
        {
            Value = new TItem[] { value };
            OnValueSetCallback(this, null);
        }
    }
}
