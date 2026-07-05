import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

const BRAND_PRIMARY = [41, 98, 255];   
const BRAND_DARK = [30, 30, 30];
const BRAND_LIGHT_GRAY = [245, 245, 245];

function addHeader(doc, plan) {

  doc.setFillColor(...BRAND_PRIMARY);
  doc.rect(0, 0, doc.internal.pageSize.getWidth(), 35, 'F');

  doc.setTextColor(255, 255, 255);
  doc.setFontSize(20);
  doc.setFont(undefined, 'bold');
  doc.text(plan.name || 'Travel Plan', 14, 18);

  doc.setFontSize(10);
  doc.setFont(undefined, 'normal');
  const dateRange = `${new Date(plan.startDate).toLocaleDateString()} - ${new Date(plan.endDate).toLocaleDateString()}`;
  doc.text(dateRange, 14, 27);

  doc.setTextColor(...BRAND_DARK);
}

function addSummary(doc, plan) {
  const startY = 45;
  doc.setFontSize(12);
  doc.setFont(undefined, 'bold');
  doc.text('View', 14, startY);

  const summaryData = [
    ['Budget', `$${(plan.plannedBudget ?? 0).toFixed(2)}`],
    ['Expenses', `$${(plan.totalExpenses ?? 0).toFixed(2)}`],
    ['Remaining', `$${(plan.remainingBudget ?? 0).toFixed(2)}`],
  ];

  autoTable(doc, {
    startY: startY + 4,
    body: summaryData,
    theme: 'plain',
    styles: { fontSize: 10, cellPadding: 2 },
    columnStyles: { 0: { fontStyle: 'bold', textColor: BRAND_DARK } },
  });

  if (plan.description) {
    let y = doc.lastAutoTable.finalY + 6;
    doc.setFont(undefined, 'normal');
    doc.setFontSize(10);
    const splitDesc = doc.splitTextToSize(plan.description, 180);
    doc.text(splitDesc, 14, y);
  }

  return doc.lastAutoTable.finalY + 14;
}

function addSectionTitle(doc, title, y) {
  if (y > 270) {
    doc.addPage();
    y = 20;
  }
  doc.setFillColor(...BRAND_LIGHT_GRAY);
  doc.rect(10, y - 6, doc.internal.pageSize.getWidth() - 20, 9, 'F');
  doc.setFontSize(12);
  doc.setFont(undefined, 'bold');
  doc.setTextColor(...BRAND_PRIMARY);
  doc.text(title, 14, y);
  doc.setTextColor(...BRAND_DARK);
  return y + 10;
}

function addDestinations(doc, destinations, y) {
  if (!destinations?.length) return y;
  y = addSectionTitle(doc, `Destinations (${destinations.length})`, y);

  const rows = destinations.map(d => [
    d.name,
    d.location || '-',
    `${new Date(d.arrivalDate).toLocaleDateString()} - ${new Date(d.departureDate).toLocaleDateString()}`,
  ]);

  autoTable(doc, {
    startY: y,
    head: [['Name', 'Location', 'Period']],
    body: rows,
    theme: 'striped',
    headStyles: { fillColor: BRAND_PRIMARY },
    styles: { fontSize: 9 },
  });

  return doc.lastAutoTable.finalY + 10;
}

function addActivities(doc, activities, y) {
  if (!activities?.length) return y;
  y = addSectionTitle(doc, `Activities (${activities.length})`, y);

  const sorted = [...activities].sort((a, b) => new Date(a.scheduledAt) - new Date(b.scheduledAt));
  const rows = sorted.map(a => [
    a.name,
    new Date(a.scheduledAt).toLocaleString(),
    a.location || '-',
  ]);

  autoTable(doc, {
    startY: y,
    head: [['Activity', 'Time', 'Location']],
    body: rows,
    theme: 'striped',
    headStyles: { fillColor: BRAND_PRIMARY },
    styles: { fontSize: 9 },
  });

  return doc.lastAutoTable.finalY + 10;
}

function addExpenses(doc, expenses, y) {
  if (!expenses?.length) return y;
  y = addSectionTitle(doc, `Expenses (${expenses.length})`, y);

  const rows = expenses.map(e => [
    e.title || e.description || '-',
    e.category || '-',
    new Date(e.date).toLocaleDateString(),
    `$${(e.amount ?? 0).toFixed(2)}`,
  ]);

  const total = expenses.reduce((sum, e) => sum + (e.amount ?? 0), 0);
  rows.push(['', '', 'Total', `$${total.toFixed(2)}`]);

  autoTable(doc, {
    startY: y,
    head: [['Item', 'Category', 'Date', 'Amount']],
    body: rows,
    theme: 'striped',
    headStyles: { fillColor: BRAND_PRIMARY },
    styles: { fontSize: 9 },
    didParseCell: (data) => {
      if (data.row.index === rows.length - 1) {
        data.cell.styles.fontStyle = 'bold';
      }
    },
  });

  return doc.lastAutoTable.finalY + 10;
}

function addChecklist(doc, items, y) {
  if (!items?.length) return y;
  y = addSectionTitle(doc, `Checklist (${items.length})`, y);

  const rows = items.map(item => [item.isCompleted ? 'Yes' : 'No', item.title]);

  autoTable(doc, {
    startY: y,
    head: [['Completed', 'Item']],
    body: rows,
    theme: 'striped',
    headStyles: { fillColor: BRAND_PRIMARY },
    styles: { fontSize: 9 },
    columnStyles: { 0: { cellWidth: 25 } },
  });

  return doc.lastAutoTable.finalY + 10;
}

function addFooter(doc) {
  const pageCount = doc.internal.getNumberOfPages();
  for (let i = 1; i <= pageCount; i++) {
    doc.setPage(i);
    doc.setFontSize(8);
    doc.setTextColor(150, 150, 150);
    doc.text(
      `Generated ${new Date().toLocaleDateString()} — Page ${i}/${pageCount}`,
      14,
      doc.internal.pageSize.getHeight() - 10
    );
  }
}

export function exportPlanToPdf(plan) {
  const doc = new jsPDF();

  addHeader(doc, plan);
  let y = addSummary(doc, plan);
  y = addDestinations(doc, plan.destinations, y);
  y = addActivities(doc, plan.activities, y);
  y = addExpenses(doc, plan.expenses, y);
  y = addChecklist(doc, plan.checklistItems, y);
  addFooter(doc);

  const fileName = `${(plan.name || 'travelplan').replace(/[^a-z0-9]/gi, '_').toLowerCase()}.pdf`;
  doc.save(fileName);
}