import { Clapperboard, Volume2 } from "lucide-react";

export const CinemaExperienceSection = () => {
  return (
    <section className="grid gap-4 sm:grid-cols-2">
      <div className="flex flex-col gap-3 rounded-2xl border border-border bg-card p-6">
        <div className="flex size-11 items-center justify-center rounded-xl bg-primary/10 text-primary">
          <Clapperboard className="size-5" />
        </div>
        <h3 className="font-heading text-lg font-semibold text-foreground">IMAX Laser</h3>
        <p className="text-sm text-muted-foreground">
          Hình ảnh sắc nét vượt trội cùng độ sáng gấp đôi rạp chiếu thông thường, đưa bạn vào từng khung hình sống động nhất.
        </p>
      </div>
      <div className="flex flex-col gap-3 rounded-2xl border border-border bg-card p-6">
        <div className="flex size-11 items-center justify-center rounded-xl bg-accent/10 text-accent">
          <Volume2 className="size-5" />
        </div>
        <h3 className="font-heading text-lg font-semibold text-foreground">Dolby Atmos</h3>
        <p className="text-sm text-muted-foreground">
          Âm thanh vòm đa chiều chân thực, tái hiện trọn vẹn từng chi tiết âm thanh như đang hòa mình vào bộ phim.
        </p>
      </div>
    </section>
  );
}