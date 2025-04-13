using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VditorMarkdownPreview
    {
        private string? previousValue;

        private bool afterFirstRender;

        private Lazy<DotNetObjectReference<object>> dotNetObjectReference = default!;

        private ElementReference outlineElementRef;

        [Inject]
        private VditorMarkdownPreviewJSModule VditorMarkdownPreviewJSModule { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public string? OutlineClass { get; set; }

        [Parameter]
        public string? OutlineStyle { get; set; }

        [Parameter]
        public bool Outline { get; set; }

        [Parameter]
        public bool RightOutline { get; set; }

        [Parameter]
        public Dictionary<string, object>? Options { get; set; }

        [Parameter]
        public EventCallback OnAfter { get; set; }

        public ElementReference Ref { get; set; }

        [JSInvokable]
        public async Task After()
        {
            if (OnAfter.HasDelegate)
            {
                await OnAfter.InvokeAsync();
            }
        }

        public async Task RenderLazyLoadingImage()
        {
            await VditorMarkdownPreviewJSModule.RenderLazyLoadingImage(Ref);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            dotNetObjectReference = new(() => DotNetObjectReference.Create<object>(this));
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (previousValue != Value)
            {
                previousValue = Value;
                await RenderingMarkdown();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                afterFirstRender = true;
                await RenderingMarkdown();
            }
        }

        private async Task RenderingMarkdown()
        {
            if (!afterFirstRender)
            {
                return;
            }

            await VditorMarkdownPreviewJSModule.PreviewVditor(dotNetObjectReference.Value, Ref, Value, Options, true, outlineElementRef);
        }
    }
}
