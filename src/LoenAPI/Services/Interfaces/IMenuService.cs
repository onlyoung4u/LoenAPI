using LoenAPI.Dtos;

namespace LoenAPI.Services.Interfaces;

public interface IMenuService
{
    Task<List<MenusDto>> Menus(int userId);

    // Task<List<MenuListDto>> MenuList();

    // Task MenuCreate(LoenMenuRequestDto menu);

    // Task MenuUpdate(LoenMenuRequestDto menu, int id);

    // Task MenuDelete(int id);
}
