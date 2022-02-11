using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShumCalcs
{
	/// <summary>
	/// Класс всех данных по расчётному блоку судовых помещений 
	/// </summary>
	public class DefShipBlock
	{
		public string Designer { get; set; } // имя проектанта
		public string ProjectNum { get; set; } // номер проекта судна
		public int CalcVarNum { get; set; } // номер варианта расчёта
		public int InstOverlapNum { get; set; } // номер установочного перекрытия

		public List<DefOverlap> Overlaps = new List<DefOverlap>(); // коллекция перекрытий блока
	}

	/// <summary>
	/// Класс всех данных по одному перекрытию 
	/// </summary>
	public class DefOverlap
	{
		public int N_overlap { get; set; } // номер перекрытия в блоке
		public double F { get; set; } // площадь перекрытия, м2
		public double P { get; set; } // периметр перекрытия, м
		public double S_mid { get; set; } // средняя толщина перекрытия, мм 
		public double S1_belt { get; set; } //  толщина первого пояса перекрытия, мм
		public double S2_belt { get; set; } // толщина второго пояса перекрытия, мм
		public int Kom1 { get; set; } // тип комингса для первого пояса перекрытия
		public int Kom2 { get; set; } // тип комингса для второго пояса перекрытия
		public double L_prod { get; set; } //  шпация продольного набора, м
		public double L_pop { get; set; } // шпация поперечного набора, м
		public string Mater { get; set; } // материал перекрытия
		public double WaterContact { get; set; } // процент соприкосновения перекрытия с водой
		public double IRG { get; set; } // момент инерции ребра жёсткости перекрытия, см4
		public double FRG { get; set; } // площадь поперечного сечения ребра жёсткости перекрытия, см2
		public double HRG { get; set; } // высота ребра жёсткости перекрытия, см

		public List<cover_layer> Cover_Layers = new List<cover_layer>(); // коллекция слоёв на перекрытии

		public main_lining MainLining = new main_lining(); // основная зашивка перекрытия
		public add_lining AddLining = new add_lining(); // дополнительная зашивка перекрытия

	}
	/// <summary>
	/// Класс материала покрытия
	/// </summary>
	public class cover_mat
	{
		public int TypeMat { get; set; } // тип материала покрытия
		public int Smin { get; set; } // минимальная толщина покрытия, мм
		public int Smax { get; set; } // максимальная толщина покрытия, мм
		public string MatOverlap { get; set; } // материал перекрытия
		public int SminOverlap { get; set; } // минимальная толщина перекрытия, мм
		public int SmaxOverlap { get; set; } // максимальная толщина перекрытия, мм

	}

	///
	/// Класс слоя покрытия
	///
	public class cover_layer
	{
		public int Num { get; set; } // номер слоя
		public cover_mat Mat = new cover_mat();  // материал покрытия, мм
		public double Smax { get; set; } // максимальная толщина покрытия, мм
		public string MatOverlap { get; set; } // материал перекрытия
		public double SminOverlap { get; set; } // минимальная толщина перекрытия, мм
		public double SmaxOverlap { get; set; } // максимальная толщина перекрытия, мм
	}

	///
	/// Класс данных по основной зашивке
	///
	public class main_lining
	{
		public int L { get; set; } // воздушный промежуток
		public double S { get; set; } // толщина зашивки, мм
		public string MatLining { get; set; } // материал зашивки
		public string T_br { get; set; } // тип звукового мостика
		public bool existZPS { get; set; } // признак наличия звукопоглощающего слоя

		public List<cover_layer> Cover_layers = new List<cover_layer>(); // коллекция слоёв зашивки
	}

	///
	/// Класс данных по дополнительной зашивке
	///
	public class add_lining
	{
		public int L { get; set; } // воздушный промежуток
		public double S { get; set; } // толщина зашивки, мм
		public string MatLining { get; set; } // материал зашивки
		public string T_br { get; set; } // тип звукового мостика
		public bool ExistZPS { get; set; } // признак наличия звукопоглощающего слоя

		public List<cover_layer> Overlaps = new List<cover_layer>(); // коллекция слоёв зашивки
	}

	///
	/// Класс данных по источнику вибрации
	/// 
	public class vibr_source
	{
		public int Num { get; set; } // номер источника
		public Dictionary<int, int> Source_ls = new Dictionary<int, int>(); // Таблица уровней шума на октавных частотах 
		public double AF { get; set; } // длина фундамента, м
		public double BF { get; set; } // ширина фундамента, м
		public double RO { get; set; } //  расстояние до точки с известным уровнем вибрации, м
		public double M_equip { get; set; } //  масса оборудования, кг
		public double M_source { get; set; } //  масса источника, кг
		public string TF { get; set; } // тип фундамента (пластинчатый,виброизолирующий)
		public bool ExistAmort { get; set; } // наличие амортизаторов 
	}

	///
	/// Класс данных по источнику воздушного шума
	/// 
	public class air_noise_source
	{
		public int Num { get; set; } // номер источника
		public Dictionary<int, int> Source_ls = new Dictionary<int, int>(); // Таблица уровней шума на октавных частотах 
		public double R { get; set; } // среднее кратчайшее расстояние от точки с известным уровнем воздушного шума до механизма, м
		public double L { get; set; } //  длина источника шума, м
		public double B { get; set; } //  ширина источника шума, м
		public double H { get; set; } //  высота источника шума, м
		public int N_overlap1 { get; set; } // номер смежного перекрытия 1;
		public int N_overlap2 { get; set; } // номер смежного перекрытия 2;
		public int N_overlap3 { get; set; } // номер смежного перекрытия 3;
		public double Rmin { get; set; } //  кратчайшее расстояние от фундамента источника до контура установочного перекрытия, м
		public double R1 { get; set; } //   расстояние от первого конца фундамента источника до контура установочного перекрытия, м
		public double R2 { get; set; } //   расстояние от второго конца фундамента источника до контура установочного перекрытия, м

	}

	///
	/// Класс данных по кожуху источника
	/// 
	public class casing
	{
		public double F { get; set; } // площадь кожуха, м2
		public double Fzpk { get; set; } // площадь звукопоглащающей конструкции в кожухе, м
		public string TA { get; set; } //  тип звукопоглощающей конструкции, м
		public double S1 { get; set; } //  толщина основной стенки кожуха,мм
		public string Mat1 { get; set; } // материал основной стенки кожуха
		public double H1 { get; set; } //   толщина слоя на основной стенке кожуха,мм
		public string Mat2 { get; set; } // материал ?? кожуха
		public double L { get; set; } //   величина воздушного промежутка,мм
		public double SZ { get; set; } //   толщина зашивки кожуха,мм
		public double MatZ { get; set; } //   материал зашивки кожуха,мм
		public bool ExistZ { get; set; } // признак наличия зашивки
		public bool ExistZPS { get; set; } // признак наличия звукопоглощающего слоя
	}

	///
	/// Класс данных по помещению источника
	/// 
	public class source_room
	{
		public int Type_room { get; set; } // тип помещения источника
		public double FP { get; set; } //  полная площадь поверхности перекрытий основного объёма помещения источника, м2
		public double Fzpk1_m { get; set; } //  площадь первой ЗПК основного объёма помещения источника, м2
		public string TA1_m { get; set; } //  тип первой звукопоглощающей конструкции основного объёма помещения источника
		public double Fzpk2_m { get; set; } //  площадь второй ЗПК основного объёма источника, м2
		public string TA2_m { get; set; } //  тип второй звукопоглощающей конструкции основного объёма помещения источника
		public double Fpr_1_2 { get; set; } //  площадь проёма первого вторичного объёма помещения источника,м2
		public double FZ_1_2 { get; set; } //   площадь ЗПК первого вторичного объёма помещения источника,м2
		public double TA_1_2 { get; set; } //   тип ЗПК первого вторичного объёма помещения источника,м2
		public double FO2 { get; set; } //  площадь проёма второго вторичного объёма помещения  источника, м2
		public double F2 { get; set; } //  полная площадь поверхности второго вторичного объёма помещения источника, м2
		public double FZ2 { get; set; } //  площадь ЗПК второго вторичного объёма помещения источника, м2
		public string TA_2 { get; set; } //  тип ЗПК звукопоглощающей конструкции второго вторичного объёма источника
		public double FOd { get; set; } // площадь проёма дополнительного вторичного объёма помещения  источника, м2
		public double ROd { get; set; } // расстояние между проёмами дополнительного вторичного объёма помещения  источника, м2
		public double Fd { get; set; } //  полная площадь поверхности дополнительного вторичного объёма помещения источника, м2
		public double FZd { get; set; } //  площадь ЗПК второго дополнительного объёма помещения источника, м2
		public string TA_d { get; set; } //  тип ЗПК звукопоглощающей конструкции дополнительного вторичного объёма источника
	}

	///
	/// Класс данных по установочному перекрытию
	/// 
	public class inst_floor
	{
		public double MD { get; set; } // масса второго дна, кг
		public double A1 { get; set; } // длина установочного перекрытия, м
		public double B { get; set; } // ширина длина установочного перекрытия, м
	}

	///
	/// Класс данных по проёмам в помещении источника
	/// 
	public class openings
	{
		public double R1_1 { get; set; } // Расстояние от расчетной точки до ближайшей вершины механизма в 1-ом вторичном объёме, м
		public double R1_2 { get; set; } // Расстояние от расчетной точки до крайней точки, лежащей на одной вертикали с точкой 1 в 1-ом вторичном объёме, м
		public double R1_3 { get; set; } // Расстояние от расчетной точки до крайней точки, лежащей на одной вертикали с точкой 4 в 1-ом вторичном объёме, м
		public double R1_4 { get; set; } // Расстояние от расчетной точки до наиболее удаленной точки механизма в 1-ом вторичном объёме, м
		public double R2_1 { get; set; } // Расстояние от расчетной точки до ближайшей вершины механизма во 2-м вторичном объёме, м
		public double R2_2 { get; set; } // Расстояние от расчетной точки до крайней точки, лежащей на одной вертикали с точкой 1 во 2-м вторичном объёме, м
		public double R2_3 { get; set; } // Расстояние от расчетной точки до крайней точки, лежащей на одной вертикали с точкой 4 во 2-м вторичном объёме, м
		public double R2_4 { get; set; } // Расстояние от расчетной точки до наиболее удаленной точки механизма в 2-ом вторичном объёме, м
	}

	///
	/// Класс расчётной точки в помещении источника
	/// 
	public class calc_point
	{
		public int Num { get; set; }     // номер источника
		public int N_point { get; set; } // номер расчётной точки
		public int N_overlap { get; set; } // номер ограждающего перекрытия 
		public double R1 { get; set; } // расстояние от расчётной точки до ближайшей вершины механизма, м
		public double R4 { get; set; } // расстояние от расчётной точки до наиболее удалённой точки механизма, м;
		public double R2 { get; set; } // расстояние от расчётной точки до крайней точки, лежащей на одной вертикали с точкой 1 
		public double R3 { get; set; } // расстояние от расчётной точки до крайней точки, лежащей на одной вертикали с точкой 4 
		public string Type_volum { get; set; } // тип объёма помещения, которому принадлежит расчётная точка
		public double Direkt_koeff { get; set; } // коэффициент направленности излучения источника (почему здесь, а не классе источника?)
		public string Type_zpk { get; set; } // тип ЗПК со стороны шумного помещения;
		public string Type_lining { get; set; } // тип зашивки на перекрытии расчётной точки; (зачем здесь?)
	}

	///
	/// Класс малошумного помещения
	/// 
	public class low_noise_room
	{
		public int Num { get; set; }     // номер помещения
		public int Type_room { get; set; } // тип малошумного помещения
		public int N_overlap { get; set; } // номер ограждающего перекрытия 

		public List<overlap_low_noise_room> Overlaps = new List<overlap_low_noise_room>(); // коллекция перекрытий малошумного помещения
	}

	///
	/// Класс перекрытия малошумного помещения
	/// 
	public class overlap_low_noise_room
	{
		public int Num { get; set; }     // номер перекрытия
		public double FK1 { get; set; } //  площадь ЗПК1 малошумного перекрытия, м2
		public int TA1 { get; set; } //     тип ЗПК1 малошумного перекрытия
		public double FK2 { get; set; } //  площадь ЗПК2 малошумного перекрытия, м2
		public int TA2 { get; set; } //     тип ЗПК2 малошумного перекрытия
		public int TZ { get; set; } //     тип зашивки малошумного перекрытия
	}

	///
	/// Класс данных разделяющего перекрытия
	/// 
	public class dividing_floor_layers
	{
		public int Num { get; set; }     // номер перекрытия
		public double L { get; set; } //  воздушный промежуток, мм
		public List<dividing_floor_layer> Overlaps = new List<dividing_floor_layer>(); // коллекция слоёв разделяющего перекрытия
		public dividing_floor_slits dividing_Floor_Slits { get; set; } // щели разделяющего перекрытия
		public dividing_floor_holes dividing_Floor_Holes { get; set; } // отверстия разделяющего перекрытия
		public dividing_floor_coffer dividing_Floor_Coffer { get; set; } // коффердам разделяющего перекрытия
	}

	///
	/// Класс одного слоя разделяющего перекрытия
	/// 
	public class dividing_floor_layer
	{
		public double H { get; set; } //  толщина слоя, мм
		public int TМ { get; set; } //   тип материала слоя
	}

	///
	/// Класс щелей разделяющего перекрытия
	/// 
	public class dividing_floor_slits
	{
		public int N { get; set; } //  количество щелей в перекрытии;
		public double F { get; set; } //  средняя площадь щели, cм2
		public int T { get; set; }   //  ширина щели, мм
	}

	///
	/// Класс отверстий разделяющего перекрытия
	/// 
	public class dividing_floor_holes
	{
		public int N { get; set; } //  количество отверстий в перекрытии;
		public double F { get; set; } //  средняя площадь отверстия, cм2
		public int T { get; set; }   //  ширина щели, мм
	}

	///
	/// Класс коффердама разделяющего перекрытия
	/// 
	public class dividing_floor_coffer
	{
		public double М { get; set; } //  масса второй стенки, cм2
		public double L { get; set; } //  воздушный промежуток, мм
		public double R { get; set; } //  плотность жидкости, г/cм3  ???
	}

	///
	/// Класс типа звукопоглощающей конструкции
	/// 
	public class sound_absorb_constr
	{
		public int Type_zpk { get; set; } //  тип ЗПК;
		public int Type_mat_zpk { get; set; } //  тип материала ЗПК;
		public double L { get; set; } //  воздушный промежуток, мм
		public double H { get; set; } //  толщина материала, мм  
		public double F { get; set; } //  площадь зашивки на одно отверстие, мм  
		public double S { get; set; } //  толщина перфорированной панели, мм;
		public double D { get; set; } //  диаметр отверстия, см;
		public double M1 { get; set; } //  масса одного см2 плёнки г/см2
		public Dictionary<int, double> Sound_absorb_koeff_tab = new Dictionary<int, double>(); // Таблица коэффициентов звукопоглощения на октавных частотах 

	}

}
