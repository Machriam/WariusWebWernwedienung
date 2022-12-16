using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WariusWebWernwedienung.Client;
public interface IModalHelper
{
    void InitializeModalService(IModalService service, IJSRuntime jSRuntime);

    Task<TResult?> ShowModal<TModal, TResult>(string title, Dictionary<string, object?> parameter,
            bool moveable = false,
        bool disableBackgroundCancel = false, bool hideCloseButton = false,
         ModalPosition position = ModalPosition.Center) where TModal : IComponent;

    Task<object?> ShowModal<TModal>(string title, Dictionary<string, object?> parameter,
            bool movable = false,
            bool disableBackgroundCancel = false, bool hideCloseButton = false,
            ModalPosition position = ModalPosition.Center) where TModal : IComponent;
}

public class ModalHelper : IModalHelper
{
    private IModalService _modalService = default!;
    private IJSRuntime _jsRuntime = default!;

    public void InitializeModalService(IModalService service, IJSRuntime jSRuntime)
    {
        _modalService = service;
        _jsRuntime = jSRuntime;
    }

    public async Task<object?> ShowModal<TModal>(string title, Dictionary<string, object?> parameter,
        bool movable = false,
        bool disableBackgroundCancel = false, bool hideCloseButton = false,
        ModalPosition position = ModalPosition.Center) where TModal : IComponent
    {
        var parameters = new ModalParameters();
        foreach (var param in parameter) parameters.Add(param.Key, param.Value);
        var options = new ModalOptions()
        {
            DisableBackgroundCancel = disableBackgroundCancel,
            HideCloseButton = hideCloseButton,
            Position = position,
            Class = movable ? "blazored-modal-draggable" : "",
            OverlayCustomClass = movable ? "blazored-disable-overlay" : "",
        };
        var modal = _modalService.Show<TModal>(title, parameters, options);
        if (movable)
        {
            await Task.Delay(1);
            await _jsRuntime.InvokeVoidAsync("window.BlazorModalExtensions.Draggable");
        }
        return (await modal.Result).Data;
    }

    public async Task<TResult?> ShowModal<TModal, TResult>(string title, Dictionary<string, object?> parameter,
        bool moveable = false,
    bool disableBackgroundCancel = false, bool hideCloseButton = false,
    ModalPosition position = ModalPosition.Center) where TModal : IComponent
    {
        return (TResult?)await ShowModal<TModal>(title, parameter, moveable, disableBackgroundCancel,
            hideCloseButton, position);
    }
}
