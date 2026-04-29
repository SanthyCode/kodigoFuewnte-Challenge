import React, { useState, useEffect } from 'react';
import styles from '../../styles/SeparataForm.module.scss';
import { useValidateSeparataMutation, useCreateSeparataMutation } from '../../store/separataApi';

// Simulamos los productos de la base de datos
const PRODUCTS = [
  { id: '3fa85f64-5717-4562-b3fc-2c963f66afa6', name: 'Laptop Pro' },
  { id: '11111111-2222-3333-4444-555555555555', name: 'Mouse Gamer' },
  { id: '99999999-8888-7777-6666-555555555555', name: 'Teclado Mecánico' }
];

export const SeparataForm = () => {
  // Estado local con el array de items (productos)
  const [formData, setFormData] = useState({
    name: '',
    startDate: '',
    endDate: '',
    items: [{ productId: PRODUCTS[0].id, promotionType: 'Percentage', promotionValue: 0 }]
  });

  const [submitStatus, setSubmitStatus] = useState<{ type: 'success' | 'error' | null, message: string }>({ type: null, message: '' });

  // Hooks de RTK Query
  const [validate, { data: validationResult, isLoading: isValidating }] = useValidateSeparataMutation();
  const [create, { isLoading: isCreating }] = useCreateSeparataMutation();

  // Validación en tiempo real (Debounce)
  useEffect(() => {
    const timeoutId = setTimeout(() => {
      if (formData.name && formData.startDate && formData.endDate) {
        validate({
          name: formData.name,
          startDate: new Date(formData.startDate).toISOString(),
          endDate: new Date(formData.endDate).toISOString(),
          items: formData.items
        });
      }
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [formData, validate]);

  const isFormValid = validationResult?.isValid ?? false;
  const serverErrors = validationResult?.errors ?? [];

  const handleItemChange = (index: number, field: string, value: string | number) => {
    if (submitStatus.type !== null) setSubmitStatus({ type: null, message: '' });

    const newItems = [...formData.items];
    newItems[index] = { ...newItems[index], [field]: value };
    setFormData({ ...formData, items: newItems });
  };

  const addItem = () => {
    if (submitStatus.type !== null) setSubmitStatus({ type: null, message: '' });
    setFormData({
      ...formData,
      items: [...formData.items, { productId: PRODUCTS[0].id, promotionType: 'Percentage', promotionValue: 0 }]
    });
  };

  const removeItem = (index: number) => {
    if (submitStatus.type !== null) setSubmitStatus({ type: null, message: '' });
    const newItems = formData.items.filter((_, i) => i !== index);
    setFormData({ ...formData, items: newItems });
  };

  // Envío del formulario
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isFormValid) return;
    setSubmitStatus({ type: null, message: '' });

    try {
      const response = await create({
        name: formData.name,
        startDate: new Date(formData.startDate).toISOString(),
        endDate: new Date(formData.endDate).toISOString(),
        items: formData.items
      }).unwrap();

      setSubmitStatus({ type: 'success', message: "✅ " + response.mensaje });

      setFormData({
        name: '', startDate: '', endDate: '',
        items: [{ productId: PRODUCTS[0].id, promotionType: 'Percentage', promotionValue: 0 }]
      });

    } catch (err) {
      // Reemplazamos el ': any' por una conversión de tipo segura de TypeScript
      const error = err as { data?: { mensaje?: string } };
      const errorMsg = error?.data?.mensaje || "Error al conectar con el servidor.";
      setSubmitStatus({ type: 'error', message: "❌ " + errorMsg });
    }
  };

  return (
    <div className={styles.formContainer}>
      <h2>Programar Nueva Separata</h2>

      {submitStatus.type === 'success' && <div className={styles.successBanner}>{submitStatus.message}</div>}
      {submitStatus.type === 'error' && <div className={styles.errorBanner}>{submitStatus.message}</div>}

      <form onSubmit={handleSubmit}>
        <div className={styles.formGroup}>
          <label>Nombre de la Campaña</label>
          <input
            type="text"
            placeholder="Ej: Promo Verano 2026"
            value={formData.name}
            onChange={e => setFormData({ ...formData, name: e.target.value })}
          />
        </div>

        <div className={styles.dateRow}>
          <div className={styles.formGroup}>
            <label>Fecha Inicio</label>
            <input
              type="datetime-local"
              value={formData.startDate}
              onChange={e => setFormData({ ...formData, startDate: e.target.value })}
            />
          </div>
          <div className={styles.formGroup}>
            <label>Fecha Fin</label>
            <input
              type="datetime-local"
              value={formData.endDate}
              onChange={e => setFormData({ ...formData, endDate: e.target.value })}
            />
          </div>
        </div>

        {/* --- SECCIÓN DINÁMICA DE PRODUCTOS --- */}
        <div className={styles.productsSection}>
          <h3>Productos en Oferta</h3>

          {formData.items.map((item, index) => (
            <div key={index} className={styles.productRow}>
              {/* Selección del Producto */}
              <select
                value={item.productId}
                onChange={e => handleItemChange(index, 'productId', e.target.value)}
              >
                {PRODUCTS.map(p => <option key={p.id} value={p.id}>{p.name}</option>)}
              </select>

              {/* Selección del TIPO de Descuento (Patrón Strategy) */}
              <select
                value={item.promotionType}
                onChange={e => handleItemChange(index, 'promotionType', e.target.value)}
              >
                <option value="Percentage">Porcentaje (%)</option>
                <option value="Direct">Monto Fijo ($)</option>
              </select>

              {/* Valor del descuento */}
              <input
                type="number"
                placeholder="Valor"
                min="0"
                value={item.promotionValue === 0 ? '' : item.promotionValue}
                onChange={e => handleItemChange(index, 'promotionValue', Number(e.target.value))}
              />

              {/* Botón para eliminar la fila (solo si hay más de 1 producto) */}
              {formData.items.length > 1 && (
                <button
                  type="button"
                  className={styles.btnRemove}
                  onClick={() => removeItem(index)}
                  title="Quitar producto"
                >
                  X
                </button>
              )}
            </div>
          ))}

          <button type="button" className={styles.btnAddProduct} onClick={addItem}>
            + Añadir otro producto
          </button>
        </div>

        {isValidating && <span style={{ color: 'gray', fontSize: '0.8rem' }}>Validando en el servidor...</span>}

        {serverErrors.length > 0 && (
          <div className={styles.errorBanner}>
            <strong>⚠️ Se encontraron problemas:</strong>
            <ul style={{ margin: '5px 0 0 20px', padding: 0 }}>
              {serverErrors.map((error, index) => (
                <li key={index}>{error}</li>
              ))}
            </ul>
          </div>
        )}

        <button
          type="submit"
          className={styles.btnSubmit}
          disabled={!isFormValid || isCreating}
          style={{ opacity: (isFormValid && !isCreating) ? 1 : 0.5, cursor: (isFormValid && !isCreating) ? 'pointer' : 'not-allowed' }}
        >
          {isCreating ? 'Guardando...' : 'Guardar Oferta'}
        </button>
      </form>
    </div>
  );
};