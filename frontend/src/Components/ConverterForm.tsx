'use client';
import React, { useState } from 'react';

const categories = ['documents', 'images', 'videos', 'audio'] as const;
const formats = {
  documents: ['Docx','Xlsx','Pptx','Pdf','Html','Markdown'],
  images: ['Jpg','Png','Gif'],
  videos: ['Mp4','Mov','Avi','Mkv','Webm','Flv','Mpeg','Wmv'],
  audio: ['Mp3','Wav']
};

type Category = typeof categories[number];

export default function ConverterForm() {
  const [category, setCategory] = useState<Category>('documents');
  const [file, setFile] = useState<File | null>(null);
  const [inputFormat, setInputFormat] = useState(formats.documents[0]);
  const [outputFormat, setOutputFormat] = useState(formats.documents[1]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Atualiza categoria e reseta formatos e arquivo
  const onCategoryChange = (cat: Category) => {
    setCategory(cat);
    setInputFormat(formats[cat][0]);
    setOutputFormat(formats[cat][1] || formats[cat][0]);
    setFile(null);
  };

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!file) return;
    setLoading(true);
    const form = new FormData();
    form.append('file', file);
    form.append('inputFormat', inputFormat);
    form.append('outputFormat', outputFormat);
    try {
      const res = await fetch(`http://localhost:5000/api/${category}/convert`, { method: 'POST', body: form });
      if (!res.ok) throw new Error('Falha na conversão');
      const blob = await res.blob();
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `output.${outputFormat.toLowerCase()}`;
      a.click();
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={onSubmit} className="w-full max-w-md p-6 bg-white rounded-xl shadow">
      <h1 className="text-2xl font-bold mb-4">SimpleFileConverter</h1>

      <label className="block mb-2">
        <span>Categoria</span>
        <select
          value={category}
          onChange={e => onCategoryChange(e.target.value as Category)}
          className="mt-1 w-full border rounded p-2"
        >
          {categories.map(c => (
            <option key={c} value={c}>{c}</option>
          ))}
        </select>
      </label>

      <label className="block mb-2">
        <span>Arquivo</span>
        <input
          type="file"
          onChange={e => setFile(e.target.files?.[0] || null)}
          className="mt-1 w-full"
        />
      </label>

      <div className="grid grid-cols-2 gap-4 mb-4">
        <label className="block">
          <span>Entrada</span>
          <select
            value={inputFormat}
            onChange={e => setInputFormat(e.target.value)}
            className="mt-1 w-full border rounded p-2"
          >
            {formats[category].map(f => <option key={f}>{f}</option>)}
          </select>
        </label>
        <label className="block">
          <span>Saída</span>
          <select
            value={outputFormat}
            onChange={e => setOutputFormat(e.target.value)}
            className="mt-1 w-full border rounded p-2"
          >
            {formats[category].map(f => <option key={f}>{f}</option>)}
          </select>
        </label>
      </div>

      {error && <p className="text-red-500 mb-2">{error}</p>}

      <button
        type="submit"
        disabled={!file || loading}
        className="w-full py-2 bg-indigo-600 text-white rounded transition disabled:opacity-50"
      >
        {loading ? 'Convertendo...' : 'Converter'}
      </button>
    </form>
  );
}