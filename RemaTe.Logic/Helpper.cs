using System.Collections.Generic;
using System.Threading.Tasks;

namespace RemaTe.Logic;
public static class Helpper {
    public static async Task<T> GetFirstOf<T>(IAsyncEnumerable<T> value) {
        await foreach (var i in value) {
            return i;
        }
        return default;
    }
}
