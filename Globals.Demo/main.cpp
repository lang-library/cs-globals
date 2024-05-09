#include <nlohmann/json.hpp>
using nljson = nlohmann::json;
#include "strconv.h"
#include <fstream>
//#include <filesystem>
#include <iomanip>

::unicode_ostream uout(std::cout, ::GetConsoleOutputCP());

int main(void)
{
    std::ifstream t(R"***(E:\.repo\base14\nuget.org\cs-globals\Globals.Demo\assets\qiita-9ea0c8fd43b61b01a8da.json)***");
    std::stringstream buffer;
    buffer << t.rdbuf();
    std::string json = buffer.str();
    //uout << json << std::endl;
    // QueryPerformanceCounter関数の1秒当たりのカウント数を取得する
    LARGE_INTEGER freq;
    QueryPerformanceFrequency(&freq);

    LARGE_INTEGER start, end;

    QueryPerformanceCounter(&start);

    // https://json.nlohmann.me/features/types/number_handling/#determine-number-types
    using json_ld = nlohmann::basic_json<std::map, std::vector, std::string, bool,
                                         std::int64_t, std::uint64_t, long double>;

    nlohmann::json x;
    for (int i=0; i<5; i++)
    {
        json_ld::parse(json);
    }

    QueryPerformanceCounter(&end);

    double time = static_cast<double>(end.QuadPart - start.QuadPart) * 1000.0 / freq.QuadPart;
    printf("time %lf[ms]\n", time);

    long double ld = 12345678901234567898.0L;
    uout << std::fixed << ld << std::endl;

    json_ld jld;
    jld = json_ld::parse("12345678901234567898");
    uout << jld.dump(2) << std::endl;
    jld = json_ld::parse("1844674407370955161.5");
    uout << jld.dump(2) << std::endl;
    ld = jld.get<long double>();
    uout << std::fixed << ld << std::endl;
    uout << std::to_string(ld) << std::endl;
    jld = json_ld::parse("18");
    uout << jld.dump(2) << std::endl;
    ld = jld.get<long double>();
    uout << std::fixed << ld << std::endl;
    uout << std::to_string(ld) << std::endl;
//
    return 0;
}
