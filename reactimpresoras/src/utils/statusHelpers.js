export const getRowColor = (p) => {
    // Si no hay serial, asumimos que no se pudo leer bien (Gris)
    if (!p.serial || p.serial === "N/A") return "table-secondary";

    // Niveles críticos (< 10%)
    const isCritical =
        p.blackToner <= 10 ||
        (p.cyanToner !== -1 && p.cyanToner <= 10) ||
        (p.magentaToner !== -1 && p.magentaToner <= 10) ||
        (p.yellowToner !== -1 && p.yellowToner <= 10);

    if (isCritical) return "table-danger";

    // Advertencia (< 50%)
    const isWarning =
        p.blackToner <= 50 ||
        (p.cyanToner !== -1 && p.cyanToner <= 50) ||
        (p.magentaToner !== -1 && p.magentaToner <= 50) ||
        (p.yellowToner !== -1 && p.yellowToner <= 50);

    if (isWarning) return "table-warning";

    // Todo OK
    return "table-success";
};